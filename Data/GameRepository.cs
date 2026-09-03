using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GameHub.Models;

namespace GameHub.Data
{
    public class GameRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public List<Game> GetGames(string search = "", string genre = "")
        {
            string sql = @"SELECT g.GameID, g.Title, g.Description, g.GenreID, ge.GenreName,
                                  g.PublisherID, p.PublisherName, g.Price, g.StockQuantity,
                                  g.ReleaseDate, g.IsActive
                           FROM Games g
                           JOIN Genres ge ON g.GenreID = ge.GenreID
                           JOIN Publishers p ON g.PublisherID = p.PublisherID
                           WHERE g.IsActive = 1
                             AND (@s = '' OR g.Title LIKE '%' + @s + '%'
                                  OR ge.GenreName LIKE '%' + @s + '%'
                                  OR p.PublisherName LIKE '%' + @s + '%')
                             AND (@g = '' OR ge.GenreName = @g)
                           ORDER BY g.Title";

            return ReadGames(sql,
                new SqlParameter("@s", search ?? string.Empty),
                new SqlParameter("@g", genre ?? string.Empty));
        }

        public Game GetGameById(int gameId)
        {
            List<Game> items = ReadGames(
                @"SELECT TOP 1 g.GameID, g.Title, g.Description, g.GenreID, ge.GenreName,
                         g.PublisherID, p.PublisherName, g.Price, g.StockQuantity,
                         g.ReleaseDate, g.IsActive
                  FROM Games g
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  WHERE g.GameID = @id AND g.IsActive = 1",
                new SqlParameter("@id", gameId));
            return items.Count == 0 ? null : items[0];
        }

        public List<Game> GetFeaturedGames(int top = 3)
        {
            if (top < 1) top = 1;
            return ReadGames(
                @"SELECT TOP (@top) g.GameID, g.Title, g.Description, g.GenreID, ge.GenreName,
                         g.PublisherID, p.PublisherName, g.Price, g.StockQuantity,
                         g.ReleaseDate, g.IsActive
                  FROM Games g
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  WHERE g.IsActive = 1
                  ORDER BY g.StockQuantity DESC, g.ReleaseDate DESC, g.GameID DESC",
                new SqlParameter("@top", top));
        }

        public List<Game> GetNewReleases(int top = 4)
        {
            if (top < 1) top = 1;
            return ReadGames(
                @"SELECT TOP (@top) g.GameID, g.Title, g.Description, g.GenreID, ge.GenreName,
                         g.PublisherID, p.PublisherName, g.Price, g.StockQuantity,
                         g.ReleaseDate, g.IsActive
                  FROM Games g
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  WHERE g.IsActive = 1
                  ORDER BY ISNULL(g.ReleaseDate, '19000101') DESC, g.GameID DESC",
                new SqlParameter("@top", top));
        }

        public List<Game> GetPopularGames(int top = 4)
        {
            if (top < 1) top = 1;
            return ReadGames(
                @"SELECT TOP (@top) g.GameID, g.Title, g.Description, g.GenreID, ge.GenreName,
                         g.PublisherID, p.PublisherName, g.Price, g.StockQuantity,
                         g.ReleaseDate, g.IsActive
                  FROM Games g
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  LEFT JOIN OrderDetails od ON g.GameID = od.GameID
                  LEFT JOIN Orders o ON od.OrderID = o.OrderID AND o.Status <> 'Cancelled'
                  WHERE g.IsActive = 1
                  GROUP BY g.GameID, g.Title, g.Description, g.GenreID, ge.GenreName,
                           g.PublisherID, p.PublisherName, g.Price, g.StockQuantity,
                           g.ReleaseDate, g.IsActive
                  ORDER BY ISNULL(SUM(CASE WHEN o.OrderID IS NULL THEN 0 ELSE od.Quantity END), 0) DESC,
                           g.GameID DESC",
                new SqlParameter("@top", top));
        }

        public DataTable GetGamesTable(string search = "")
        {
            return _db.ExecuteQuery(
                @"SELECT g.GameID AS [ID], g.Title, ge.GenreName AS [Genre],
                         p.PublisherName AS [Publisher], g.Price, g.StockQuantity AS [Stock],
                         g.ReleaseDate AS [Release Date], g.Description
                  FROM Games g
                  JOIN Genres ge ON g.GenreID = ge.GenreID
                  JOIN Publishers p ON g.PublisherID = p.PublisherID
                  WHERE g.IsActive = 1
                    AND (@s = '' OR g.Title LIKE '%' + @s + '%'
                         OR ge.GenreName LIKE '%' + @s + '%'
                         OR p.PublisherName LIKE '%' + @s + '%')
                  ORDER BY g.GameID",
                new SqlParameter("@s", search ?? string.Empty));
        }

        public DataTable GetGenres()
        {
            return _db.ExecuteQuery("SELECT GenreID, GenreName FROM Genres ORDER BY GenreName");
        }

        public DataTable GetPublishers()
        {
            return _db.ExecuteQuery("SELECT PublisherID, PublisherName FROM Publishers ORDER BY PublisherName");
        }

        public void AddGame(string title, string desc, int genreId, int publisherId,
                            decimal price, int stock, DateTime? releaseDate)
        {
            _db.ExecuteNonQuery(
                @"INSERT INTO Games
                    (Title, Description, GenreID, PublisherID, Price, StockQuantity, ReleaseDate, IsActive)
                  VALUES (@t, @d, @g, @p, @pr, @st, @rd, 1)",
                new SqlParameter("@t", title.Trim()),
                new SqlParameter("@d", (object)(desc ?? string.Empty).Trim()),
                new SqlParameter("@g", genreId),
                new SqlParameter("@p", publisherId),
                new SqlParameter("@pr", price),
                new SqlParameter("@st", stock),
                new SqlParameter("@rd", releaseDate.HasValue ? (object)releaseDate.Value.Date : DBNull.Value));
        }

        public void UpdateGame(int id, string title, string desc, int genreId, int publisherId,
                               decimal price, int stock, DateTime? releaseDate)
        {
            _db.ExecuteNonQuery(
                @"UPDATE Games
                  SET Title = @t, Description = @d, GenreID = @g, PublisherID = @p,
                      Price = @pr, StockQuantity = @st, ReleaseDate = @rd
                  WHERE GameID = @id",
                new SqlParameter("@t", title.Trim()),
                new SqlParameter("@d", (object)(desc ?? string.Empty).Trim()),
                new SqlParameter("@g", genreId),
                new SqlParameter("@p", publisherId),
                new SqlParameter("@pr", price),
                new SqlParameter("@st", stock),
                new SqlParameter("@rd", releaseDate.HasValue ? (object)releaseDate.Value.Date : DBNull.Value),
                new SqlParameter("@id", id));
        }

        public void DeleteGame(int id)
        {
            _db.ExecuteNonQuery(
                "UPDATE Games SET IsActive = 0 WHERE GameID = @id",
                new SqlParameter("@id", id));
        }

        private List<Game> ReadGames(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = _db.ExecuteQuery(sql, parameters);
            List<Game> list = new List<Game>();
            foreach (DataRow r in dt.Rows) list.Add(Map(r));
            return list;
        }

        private static Game Map(DataRow r)
        {
            return new Game
            {
                GameID = Convert.ToInt32(r["GameID"]),
                Title = Convert.ToString(r["Title"]),
                Description = r["Description"] == DBNull.Value ? string.Empty : Convert.ToString(r["Description"]),
                GenreID = Convert.ToInt32(r["GenreID"]),
                GenreName = Convert.ToString(r["GenreName"]),
                PublisherID = Convert.ToInt32(r["PublisherID"]),
                PublisherName = Convert.ToString(r["PublisherName"]),
                Price = Convert.ToDecimal(r["Price"]),
                StockQuantity = Convert.ToInt32(r["StockQuantity"]),
                ReleaseDate = r["ReleaseDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["ReleaseDate"]),
                IsActive = Convert.ToBoolean(r["IsActive"])
            };
        }
    }
}
