using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GameHub.Models;

namespace GameHub.Data
{
    public class OrderRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        /// <summary>
        /// Create an order, validate stock, optionally charge the wallet, create payment,
        /// add loyalty points, deduct stock and generate keys in ONE database transaction.
        /// This prevents wallet balance and order data from becoming out of sync.
        /// </summary>
        public int PlaceOrder(int customerId, List<CartItem> cart, decimal total,
                              string paymentMethod, out List<string> keys)
        {
            if (cart == null || cart.Count == 0)
                throw new InvalidOperationException("The cart is empty.");
            if (total < 0)
                throw new InvalidOperationException("Order total cannot be negative.");

            keys = new List<string>();
            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        // Validate stock with update locks so another checkout cannot oversell.
                        foreach (CartItem item in cart)
                        {
                            if (item == null || item.Game == null || item.Quantity <= 0)
                                throw new InvalidOperationException("The cart contains an invalid item.");

                            int available;
                            using (SqlCommand cmd = new SqlCommand(
                                @"SELECT StockQuantity
                                  FROM Games WITH (UPDLOCK, ROWLOCK)
                                  WHERE GameID = @g AND IsActive = 1", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@g", item.Game.GameID);
                                object o = cmd.ExecuteScalar();
                                if (o == null || o == DBNull.Value)
                                    throw new InvalidOperationException("A game in your cart is no longer available.");
                                available = Convert.ToInt32(o);
                            }

                            if (available < item.Quantity)
                                throw new InvalidOperationException(
                                    item.Game.Title + " only has " + available + " copy/copies left in stock.");
                        }

                        // Wallet payment is charged inside the same transaction as the order.
                        if (string.Equals(paymentMethod, "Wallet", StringComparison.OrdinalIgnoreCase))
                        {
                            decimal balance;
                            using (SqlCommand cmd = new SqlCommand(
                                "SELECT Balance FROM Wallet WITH (UPDLOCK, ROWLOCK) WHERE UserID = @u", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@u", customerId);
                                object o = cmd.ExecuteScalar();
                                if (o == null || o == DBNull.Value) throw new InvalidOperationException("Wallet not found.");
                                balance = Convert.ToDecimal(o);
                            }

                            if (balance < total)
                                throw new InvalidOperationException(
                                    "Insufficient wallet balance. Available: Tk " + balance.ToString("N2"));

                            using (SqlCommand cmd = new SqlCommand(
                                "UPDATE Wallet SET Balance = Balance - @a WHERE UserID = @u", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@a", total);
                                cmd.Parameters.AddWithValue("@u", customerId);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"INSERT INTO WalletTransactions (WalletID, TxType, Amount)
                                  SELECT WalletID, 'Spend', -@a FROM Wallet WHERE UserID = @u", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@a", total);
                                cmd.Parameters.AddWithValue("@u", customerId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        int orderId;
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO Orders (CustomerID, OrderDate, TotalAmount, Status)
                              VALUES (@c, GETDATE(), @t, 'Paid');
                              SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@c", customerId);
                            cmd.Parameters.AddWithValue("@t", total);
                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (CartItem item in cart)
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                @"INSERT INTO OrderDetails (OrderID, GameID, Quantity, UnitPrice, Subtotal)
                                  VALUES (@o, @g, @q, @u, @s)", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@o", orderId);
                                cmd.Parameters.AddWithValue("@g", item.Game.GameID);
                                cmd.Parameters.AddWithValue("@q", item.Quantity);
                                cmd.Parameters.AddWithValue("@u", item.Game.Price);
                                cmd.Parameters.AddWithValue("@s", item.Subtotal);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                "UPDATE Games SET StockQuantity = StockQuantity - @q WHERE GameID = @g", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@q", item.Quantity);
                                cmd.Parameters.AddWithValue("@g", item.Game.GameID);
                                cmd.ExecuteNonQuery();
                            }

                            for (int i = 0; i < item.Quantity; i++)
                            {
                                string code = GenerateKey();
                                keys.Add(item.Game.Title + " : " + code);

                                using (SqlCommand cmd = new SqlCommand(
                                    @"INSERT INTO GameKeys (OrderID, GameID, ActivationCode)
                                      VALUES (@o, @g, @k)", con, tx))
                                {
                                    cmd.Parameters.AddWithValue("@o", orderId);
                                    cmd.Parameters.AddWithValue("@g", item.Game.GameID);
                                    cmd.Parameters.AddWithValue("@k", code);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO Payments (OrderID, Amount, PaymentMethod, Status)
                              VALUES (@o, @a, @m, 'Completed')", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@o", orderId);
                            cmd.Parameters.AddWithValue("@a", total);
                            cmd.Parameters.AddWithValue("@m", paymentMethod ?? "Unknown");
                            cmd.ExecuteNonQuery();
                        }

                        int points = (int)(total / 100m) * 10;
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Wallet SET LoyaltyPoints = LoyaltyPoints + @p WHERE UserID = @c", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@p", points);
                            cmd.Parameters.AddWithValue("@c", customerId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return orderId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public DataTable GetRecentOrders(int top = 8)
        {
            if (top < 1) top = 1;
            return _db.ExecuteQuery(
                @"SELECT TOP (@top) o.OrderID AS [Order #], u.Username AS [Customer],
                         o.OrderDate AS [Date], o.TotalAmount AS [Amount], o.Status
                  FROM Orders o
                  JOIN Users u ON o.CustomerID = u.UserID
                  ORDER BY o.OrderID DESC",
                new SqlParameter("@top", top));
        }

        public DataTable GetOrders(string search = "")
        {
            return _db.ExecuteQuery(
                @"SELECT o.OrderID AS [Order ID], u.Username AS [Customer],
                         u.FullName AS [Customer Name], o.OrderDate AS [Date],
                         o.TotalAmount AS [Total], o.Status,
                         ISNULL(s.Username, '-') AS [Processed By]
                  FROM Orders o
                  JOIN Users u ON o.CustomerID = u.UserID
                  LEFT JOIN Users s ON o.ProcessedByStaffID = s.UserID
                  WHERE @s = '' OR CAST(o.OrderID AS VARCHAR(20)) LIKE '%' + @s + '%'
                                OR u.Username LIKE '%' + @s + '%'
                                OR u.FullName LIKE '%' + @s + '%'
                                OR o.Status LIKE '%' + @s + '%'
                  ORDER BY o.OrderID DESC",
                new SqlParameter("@s", search ?? string.Empty));
        }

        public DataTable GetOrderDetails(int orderId)
        {
            return _db.ExecuteQuery(
                @"SELECT g.Title AS [Game], od.Quantity AS [Qty], od.UnitPrice AS [Unit Price],
                         od.Subtotal AS [Subtotal]
                  FROM OrderDetails od
                  JOIN Games g ON od.GameID = g.GameID
                  WHERE od.OrderID = @o
                  ORDER BY od.OrderDetailID",
                new SqlParameter("@o", orderId));
        }

        public void UpdateOrderStatus(int orderId, string status, int? processedByStaffId)
        {
            string[] allowed = { "Pending", "Paid", "Completed", "Cancelled" };
            bool valid = false;
            foreach (string s in allowed)
                if (string.Equals(s, status, StringComparison.OrdinalIgnoreCase)) valid = true;
            if (!valid) throw new ArgumentException("Invalid order status.");

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        string currentStatus;
                        int customerId;
                        decimal total;
                        using (SqlCommand cmd = new SqlCommand(
                            @"SELECT CustomerID, TotalAmount, Status
                              FROM Orders WITH (UPDLOCK, ROWLOCK)
                              WHERE OrderID = @o", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@o", orderId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read()) throw new InvalidOperationException("Order not found.");
                                customerId = reader.GetInt32(0);
                                total = reader.GetDecimal(1);
                                currentStatus = reader.GetString(2);
                            }
                        }

                        if (string.Equals(currentStatus, "Cancelled", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("A cancelled order cannot be reopened automatically.");

                        if (string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(currentStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
                        {
                            // Return purchased stock.
                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE g
                                  SET g.StockQuantity = g.StockQuantity + od.Quantity
                                  FROM Games g
                                  JOIN OrderDetails od ON g.GameID = od.GameID
                                  WHERE od.OrderID = @o", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@o", orderId);
                                cmd.ExecuteNonQuery();
                            }

                            string paymentMethod = string.Empty;
                            using (SqlCommand cmd = new SqlCommand(
                                "SELECT TOP 1 PaymentMethod FROM Payments WHERE OrderID = @o", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@o", orderId);
                                object method = cmd.ExecuteScalar();
                                if (method != null && method != DBNull.Value) paymentMethod = Convert.ToString(method);
                            }

                            // Wallet orders receive an automatic wallet refund.
                            if (string.Equals(paymentMethod, "Wallet", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "UPDATE Wallet SET Balance = Balance + @a WHERE UserID = @u", con, tx))
                                {
                                    cmd.Parameters.AddWithValue("@a", total);
                                    cmd.Parameters.AddWithValue("@u", customerId);
                                    cmd.ExecuteNonQuery();
                                }

                                using (SqlCommand cmd = new SqlCommand(
                                    @"INSERT INTO WalletTransactions (WalletID, TxType, Amount)
                                      SELECT WalletID, 'Refund', @a FROM Wallet WHERE UserID = @u", con, tx))
                                {
                                    cmd.Parameters.AddWithValue("@a", total);
                                    cmd.Parameters.AddWithValue("@u", customerId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            int points = (int)(total / 100m) * 10;
                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE Wallet
                                  SET LoyaltyPoints = CASE WHEN LoyaltyPoints >= @p THEN LoyaltyPoints - @p ELSE 0 END
                                  WHERE UserID = @u", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@p", points);
                                cmd.Parameters.AddWithValue("@u", customerId);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                "UPDATE Payments SET Status = 'Refunded' WHERE OrderID = @o", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@o", orderId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"UPDATE Orders
                              SET Status = @st,
                                  ProcessedByStaffID = CASE WHEN @sid IS NULL THEN ProcessedByStaffID ELSE @sid END
                              WHERE OrderID = @o", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@st", status);
                            cmd.Parameters.AddWithValue("@sid",
                                processedByStaffId.HasValue ? (object)processedByStaffId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@o", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public DataTable GetPayments(string search = "")
        {
            return _db.ExecuteQuery(
                @"SELECT p.PaymentID AS [Payment ID], p.OrderID AS [Order ID],
                         u.Username AS [Customer], p.Amount, p.PaymentMethod AS [Method],
                         p.PaymentDate AS [Date], p.Status
                  FROM Payments p
                  JOIN Orders o ON p.OrderID = o.OrderID
                  JOIN Users u ON o.CustomerID = u.UserID
                  WHERE @s = '' OR CAST(p.PaymentID AS VARCHAR(20)) LIKE '%' + @s + '%'
                                OR CAST(p.OrderID AS VARCHAR(20)) LIKE '%' + @s + '%'
                                OR u.Username LIKE '%' + @s + '%'
                                OR p.PaymentMethod LIKE '%' + @s + '%'
                                OR p.Status LIKE '%' + @s + '%'
                  ORDER BY p.PaymentID DESC",
                new SqlParameter("@s", search ?? string.Empty));
        }

        private static string GenerateKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            char[] buf = new char[15];
            for (int i = 0; i < buf.Length; i++) buf[i] = chars[rnd.Next(chars.Length)];
            string s = new string(buf);
            return s.Substring(0, 5) + "-" + s.Substring(5, 5) + "-" + s.Substring(10, 5);
        }
    }
}
