using System;
using System.Data;

namespace GameHub.Data
{
    public class ReportRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        /// <summary>
        /// Revenue includes both Paid and Completed orders. This avoids losing revenue
        /// from reports after staff finish processing a paid order.
        /// </summary>
        public DataTable RevenueByGenre()
        {
            return _db.ExecuteQuery(
                @"SELECT ge.GenreName,
                         SUM(od.Subtotal) AS Revenue,
                         COUNT(DISTINCT o.OrderID) AS Orders
                  FROM OrderDetails od
                  JOIN Games g ON od.GameID = g.GameID
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Orders o ON od.OrderID = o.OrderID
                  WHERE o.Status IN ('Paid', 'Completed')
                  GROUP BY ge.GenreName
                  ORDER BY Revenue DESC");
        }

        public int TotalGames()
        {
            return Convert.ToInt32(_db.ExecuteScalar(
                "SELECT COUNT(*) FROM Games WHERE IsActive = 1"));
        }

        public int TotalOrders()
        {
            return Convert.ToInt32(_db.ExecuteScalar(
                "SELECT COUNT(*) FROM Orders"));
        }

        public decimal TotalRevenue()
        {
            object o = _db.ExecuteScalar(
                "SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE Status IN ('Paid', 'Completed')");
            return Convert.ToDecimal(o);
        }

        public int ActiveUsers()
        {
            return Convert.ToInt32(_db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE IsActive = 1"));
        }
    }
}
