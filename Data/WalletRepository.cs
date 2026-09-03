using System;
using System.Data;
using System.Data.SqlClient;

namespace GameHub.Data
{
    public class WalletRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public decimal GetBalance(int userId)
        {
            object o = _db.ExecuteScalar(
                "SELECT Balance FROM Wallet WHERE UserID = @u",
                new SqlParameter("@u", userId));
            return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        }

        public int GetPoints(int userId)
        {
            object o = _db.ExecuteScalar(
                "SELECT LoyaltyPoints FROM Wallet WHERE UserID = @u",
                new SqlParameter("@u", userId));
            return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
        }

        /// <summary>Add a positive amount to the wallet and record the top-up atomically.</summary>
        public void TopUp(int userId, decimal amount, string method)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("amount", "Top-up amount must be greater than zero.");

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int walletId;
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT WalletID FROM Wallet WITH (UPDLOCK, ROWLOCK) WHERE UserID = @u", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", userId);
                            object o = cmd.ExecuteScalar();
                            if (o == null || o == DBNull.Value)
                                throw new InvalidOperationException("Wallet not found for this user.");
                            walletId = Convert.ToInt32(o);
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Wallet SET Balance = Balance + @a WHERE WalletID = @w", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", amount);
                            cmd.Parameters.AddWithValue("@w", walletId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO WalletTransactions (WalletID, TxType, Amount)
                              VALUES (@w, @type, @a)", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@w", walletId);
                            cmd.Parameters.AddWithValue("@type",
                                string.IsNullOrWhiteSpace(method) ? "Topup" : method.Trim());
                            cmd.Parameters.AddWithValue("@a", amount);
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

        /// <summary>
        /// Stand-alone wallet spending helper. Checkout uses OrderRepository so payment + order remain atomic.
        /// </summary>
        public void Spend(int userId, decimal amount, string reason)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException("amount", "Spend amount must be greater than zero.");

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        decimal balance;
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT Balance FROM Wallet WITH (UPDLOCK, ROWLOCK) WHERE UserID = @u", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", userId);
                            object o = cmd.ExecuteScalar();
                            if (o == null || o == DBNull.Value) throw new InvalidOperationException("Wallet not found.");
                            balance = Convert.ToDecimal(o);
                        }

                        if (balance < amount) throw new InvalidOperationException("Insufficient wallet balance.");

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Wallet SET Balance = Balance - @a WHERE UserID = @u", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", amount);
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO WalletTransactions (WalletID, TxType, Amount)
                              SELECT WalletID, @type, -@a FROM Wallet WHERE UserID = @u", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", amount);
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.Parameters.AddWithValue("@type", string.IsNullOrWhiteSpace(reason) ? "Spend" : reason);
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

        public DataTable GetTransactions(int userId)
        {
            return _db.ExecuteQuery(
                @"SELECT wt.TxDate AS [Date], wt.TxType AS [Type], wt.Amount
                  FROM WalletTransactions wt
                  JOIN Wallet w ON wt.WalletID = w.WalletID
                  WHERE w.UserID = @u
                  ORDER BY wt.TxID DESC",
                new SqlParameter("@u", userId));
        }
    }
}
