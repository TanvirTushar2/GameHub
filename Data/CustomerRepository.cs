using System;
using System.Data;
using System.Data.SqlClient;

namespace GameHub.Data
{
    /// <summary>Customer-only features that use the existing Wishlist, Reviews, Orders and GameKeys tables.</summary>
    public class CustomerRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public DataTable GetLibrary(int userId)
        {
            return _db.ExecuteQuery(
                @"SELECT g.GameID AS [Game ID], g.Title, ge.GenreName AS [Genre],
                         p.PublisherName AS [Publisher], o.OrderDate AS [Purchased],
                         gk.ActivationCode AS [Activation Key]
                  FROM GameKeys gk
                  JOIN Orders o ON gk.OrderID = o.OrderID
                  JOIN Games g ON gk.GameID = g.GameID
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  WHERE o.CustomerID = @u AND o.Status <> 'Cancelled'
                  ORDER BY o.OrderDate DESC, g.Title",
                new SqlParameter("@u", userId));
        }

        public int GetLibraryCount(int userId)
        {
            object o = _db.ExecuteScalar(
                @"SELECT COUNT(DISTINCT gk.GameID)
                  FROM GameKeys gk
                  JOIN Orders o ON gk.OrderID = o.OrderID
                  WHERE o.CustomerID = @u AND o.Status <> 'Cancelled'",
                new SqlParameter("@u", userId));
            return Convert.ToInt32(o);
        }

        public int GetOrderCount(int userId)
        {
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Orders WHERE CustomerID = @u AND Status <> 'Cancelled'",
                new SqlParameter("@u", userId));
            return Convert.ToInt32(o);
        }

        public DataTable GetWishlist(int userId)
        {
            return _db.ExecuteQuery(
                @"SELECT g.GameID AS [Game ID], g.Title, ge.GenreName AS [Genre],
                         p.PublisherName AS [Publisher], g.Price, g.StockQuantity AS [Stock],
                         w.AlertOnSale AS [Sale Alert]
                  FROM Wishlist w
                  JOIN Games g ON w.GameID = g.GameID
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  WHERE w.UserID = @u AND g.IsActive = 1
                  ORDER BY g.Title",
                new SqlParameter("@u", userId));
        }

        public int GetWishlistCount(int userId)
        {
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Wishlist WHERE UserID = @u",
                new SqlParameter("@u", userId));
            return Convert.ToInt32(o);
        }

        public bool IsInWishlist(int userId, int gameId)
        {
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Wishlist WHERE UserID = @u AND GameID = @g",
                new SqlParameter("@u", userId),
                new SqlParameter("@g", gameId));
            return Convert.ToInt32(o) > 0;
        }

        public void AddToWishlist(int userId, int gameId)
        {
            _db.ExecuteNonQuery(
                @"IF NOT EXISTS (SELECT 1 FROM Wishlist WHERE UserID = @u AND GameID = @g)
                      INSERT INTO Wishlist (UserID, GameID, AlertOnSale) VALUES (@u, @g, 1)",
                new SqlParameter("@u", userId),
                new SqlParameter("@g", gameId));
        }

        public void RemoveFromWishlist(int userId, int gameId)
        {
            _db.ExecuteNonQuery(
                "DELETE FROM Wishlist WHERE UserID = @u AND GameID = @g",
                new SqlParameter("@u", userId),
                new SqlParameter("@g", gameId));
        }

        public DataTable GetReviewsForGame(int gameId)
        {
            return _db.ExecuteQuery(
                @"SELECT u.Username AS [Player], r.Rating, r.Comment, r.CreatedAt AS [Date]
                  FROM Reviews r
                  JOIN Users u ON r.UserID = u.UserID
                  WHERE r.GameID = @g
                  ORDER BY r.CreatedAt DESC",
                new SqlParameter("@g", gameId));
        }

        public DataTable GetUserReview(int userId, int gameId)
        {
            return _db.ExecuteQuery(
                "SELECT TOP 1 Rating, Comment FROM Reviews WHERE UserID = @u AND GameID = @g ORDER BY ReviewID DESC",
                new SqlParameter("@u", userId),
                new SqlParameter("@g", gameId));
        }

        public void SaveReview(int userId, int gameId, int rating, string comment)
        {
            if (rating < 1 || rating > 5) throw new ArgumentOutOfRangeException("rating");

            object owns = _db.ExecuteScalar(
                @"SELECT COUNT(*)
                  FROM GameKeys gk
                  JOIN Orders o ON gk.OrderID = o.OrderID
                  WHERE o.CustomerID = @u AND gk.GameID = @g AND o.Status <> 'Cancelled'",
                new SqlParameter("@u", userId),
                new SqlParameter("@g", gameId));

            if (Convert.ToInt32(owns) == 0)
                throw new InvalidOperationException("Only customers who own this game can review it.");

            _db.ExecuteNonQuery(
                @"IF EXISTS (SELECT 1 FROM Reviews WHERE UserID = @u AND GameID = @g)
                  BEGIN
                      UPDATE Reviews
                      SET Rating = @r, Comment = @c, CreatedAt = GETDATE()
                      WHERE UserID = @u AND GameID = @g;
                  END
                  ELSE
                  BEGIN
                      INSERT INTO Reviews (UserID, GameID, Rating, Comment)
                      VALUES (@u, @g, @r, @c);
                  END",
                new SqlParameter("@u", userId),
                new SqlParameter("@g", gameId),
                new SqlParameter("@r", rating),
                new SqlParameter("@c", (object)(comment ?? string.Empty).Trim()));
        }
    }
}
