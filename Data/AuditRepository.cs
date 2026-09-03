using System;
using System.Data;
using System.Data.SqlClient;

namespace GameHub.Data
{
    /// <summary>
    /// Records important actions in the SystemLog table and reads them back.
    /// Used by the Super Admin "System Log" screen (owner oversight).
    /// </summary>
    public class AuditRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public void Log(int userId, string username, string action, string details)
        {
            _db.ExecuteNonQuery(
                @"INSERT INTO SystemLog (UserID, Username, Action, Details)
                  VALUES (@uid, @un, @ac, @de)",
                new SqlParameter("@uid", userId <= 0 ? (object)DBNull.Value : userId),
                new SqlParameter("@un", (object)(username ?? "system")),
                new SqlParameter("@ac", (object)(action ?? "")),
                new SqlParameter("@de", (object)(details ?? "")));
        }

        /// <summary>Convenience: log the action for whoever is currently signed in.</summary>
        public static void Write(string action, string details)
        {
            try
            {
                Models.User u = Program.CurrentUser;
                new AuditRepository().Log(
                    u == null ? 0 : u.UserID,
                    u == null ? "system" : u.Username,
                    action,
                    details);
            }
            catch
            {
                // Auditing must never break a real action.
            }
        }

        public DataTable GetRecent(int top = 300)
        {
            return _db.ExecuteQuery(
                @"SELECT TOP (" + top + @")
                         CONVERT(VARCHAR(19), CreatedAt, 120) AS [When],
                         Username AS [User],
                         Action,
                         Details
                  FROM SystemLog
                  ORDER BY LogID DESC");
        }
    }
}
