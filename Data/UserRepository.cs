using System;
using System.Data;
using System.Data.SqlClient;
using GameHub.Models;

namespace GameHub.Data
{
    public class UserRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        /// <summary>
        /// Validate login by username OR email. Returns the matching role-specific User or null.
        /// Existing SHA-256 authentication is preserved.
        /// </summary>
        public User Login(string identifier, string password)
        {
            string sql = @"SELECT TOP 1 UserID, Username, Role, FullName, Email, Phone, IsActive
                           FROM Users
                           WHERE (Username = @id OR Email = @id)
                             AND PasswordHash = @p
                             AND IsActive = 1";

            DataTable dt = _db.ExecuteQuery(sql,
                new SqlParameter("@id", (identifier ?? string.Empty).Trim()),
                new SqlParameter("@p", Security.Hash(password)));

            if (dt.Rows.Count == 0) return null;
            return MapUser(dt.Rows[0]);
        }

        public bool UsernameExists(string username)
        {
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Username = @u",
                new SqlParameter("@u", (username ?? string.Empty).Trim()));
            return Convert.ToInt32(o) > 0;
        }

        public bool EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Email = @e",
                new SqlParameter("@e", email.Trim()));
            return Convert.ToInt32(o) > 0;
        }

        public bool EmailUsedByAnotherUser(string email, int userId)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Email = @e AND UserID <> @id",
                new SqlParameter("@e", email.Trim()),
                new SqlParameter("@id", userId));
            return Convert.ToInt32(o) > 0;
        }

        public bool UsernameUsedByAnotherUser(string username, int userId)
        {
            object o = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Username = @u AND UserID <> @id",
                new SqlParameter("@u", (username ?? string.Empty).Trim()),
                new SqlParameter("@id", userId));
            return Convert.ToInt32(o) > 0;
        }

        /// <summary>Register a new Customer and create a wallet in the same transaction.</summary>
        public void Register(string username, string password, string fullName, string email, string phone)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int userId;
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO Users (Username, PasswordHash, Role, FullName, Email, Phone)
                              VALUES (@u, @p, 'Customer', @f, @e, @ph);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", username.Trim());
                            cmd.Parameters.AddWithValue("@p", Security.Hash(password));
                            cmd.Parameters.AddWithValue("@f", fullName.Trim());
                            cmd.Parameters.AddWithValue("@e", (object)(email ?? string.Empty).Trim());
                            cmd.Parameters.AddWithValue("@ph", (object)(phone ?? string.Empty).Trim());
                            userId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Wallet (UserID, Balance, LoyaltyPoints) VALUES (@u, 0, 0)", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", userId);
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

        public DataTable GetAllUsers()
        {
            return _db.ExecuteQuery(
                @"SELECT UserID, Username, Role, FullName, Email, Phone, IsActive, CreatedAt
                  FROM Users
                  ORDER BY UserID");
        }

        public DataTable GetCustomersForSupport(string search = "")
        {
            return _db.ExecuteQuery(
                @"SELECT UserID AS [ID], Username, FullName AS [Full Name], Email, Phone,
                         CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS [Status],
                         CreatedAt AS [Joined]
                  FROM Users
                  WHERE Role = 'Customer'
                    AND (@s = '' OR Username LIKE '%' + @s + '%' OR FullName LIKE '%' + @s + '%'
                         OR Email LIKE '%' + @s + '%' OR Phone LIKE '%' + @s + '%')
                  ORDER BY FullName, Username",
                new SqlParameter("@s", search ?? string.Empty));
        }

        public User GetUserById(int userId)
        {
            DataTable dt = _db.ExecuteQuery(
                @"SELECT TOP 1 UserID, Username, Role, FullName, Email, Phone, IsActive
                  FROM Users WHERE UserID = @id",
                new SqlParameter("@id", userId));
            return dt.Rows.Count == 0 ? null : MapUser(dt.Rows[0]);
        }

        public DataTable FindAccountForRecovery(string identifier)
        {
            return _db.ExecuteQuery(
                @"SELECT TOP 1 Username, Email, Phone, IsActive
                  FROM Users
                  WHERE Username = @id OR Email = @id",
                new SqlParameter("@id", (identifier ?? string.Empty).Trim()));
        }

        /// <summary>Create an Admin/Staff/Customer account. Customer accounts also receive a wallet.</summary>
        // ---- Authoritative permission guard (data layer) ----
        // Only a Super Admin may create or modify Admin / Super Admin accounts.
        private static bool CurrentIsSuper()
        {
            return Program.CurrentUser != null &&
                   Program.CurrentUser.Role == "SuperAdmin";
        }

        private static bool IsPrivileged(string role)
        {
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        private static void GuardCanManage(string targetRole)
        {
            if (!CurrentIsSuper() && IsPrivileged(targetRole))
                throw new InvalidOperationException(
                    "Only a Super Admin can create or manage Admin accounts.");
        }

        private string GetRoleById(int id)
        {
            return Convert.ToString(
                _db.ExecuteScalar(
                    "SELECT Role FROM Users WHERE UserID = @id",
                    new SqlParameter("@id", id)));
        }

        public void AddUser(string username, string password, string role, string fullName, string email, string phone)
        {
            GuardCanManage(role);

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int userId;
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO Users (Username, PasswordHash, Role, FullName, Email, Phone)
                              VALUES (@u, @p, @r, @f, @e, @ph);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", username.Trim());
                            cmd.Parameters.AddWithValue("@p", Security.Hash(password));
                            cmd.Parameters.AddWithValue("@r", role);
                            cmd.Parameters.AddWithValue("@f", fullName.Trim());
                            cmd.Parameters.AddWithValue("@e", (object)(email ?? string.Empty).Trim());
                            cmd.Parameters.AddWithValue("@ph", (object)(phone ?? string.Empty).Trim());
                            userId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        if (string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase))
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "INSERT INTO Wallet (UserID, Balance, LoyaltyPoints) VALUES (@u, 0, 0)", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@u", userId);
                                cmd.ExecuteNonQuery();
                            }
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

        public void UpdateUser(int id, string username, string role, string fullName, string email, string phone, bool isActive)
        {
            GuardCanManage(GetRoleById(id));
            GuardCanManage(role);

            _db.ExecuteNonQuery(
                @"UPDATE Users
                  SET Username = @un, Role = @r, FullName = @f, Email = @e, Phone = @ph, IsActive = @a
                  WHERE UserID = @id;

                  IF @r = 'Customer' AND NOT EXISTS (SELECT 1 FROM Wallet WHERE UserID = @id)
                      INSERT INTO Wallet (UserID, Balance, LoyaltyPoints) VALUES (@id, 0, 0);",
                new SqlParameter("@un", username.Trim()),
                new SqlParameter("@r", role),
                new SqlParameter("@f", fullName.Trim()),
                new SqlParameter("@e", (object)(email ?? string.Empty).Trim()),
                new SqlParameter("@ph", (object)(phone ?? string.Empty).Trim()),
                new SqlParameter("@a", isActive),
                new SqlParameter("@id", id));
        }

        public void UpdateProfile(int userId, string fullName, string email, string phone)
        {
            _db.ExecuteNonQuery(
                @"UPDATE Users SET FullName = @f, Email = @e, Phone = @ph WHERE UserID = @id",
                new SqlParameter("@f", fullName.Trim()),
                new SqlParameter("@e", (object)(email ?? string.Empty).Trim()),
                new SqlParameter("@ph", (object)(phone ?? string.Empty).Trim()),
                new SqlParameter("@id", userId));
        }

        public void ResetPassword(int userId, string newPassword)
        {
            _db.ExecuteNonQuery(
                "UPDATE Users SET PasswordHash = @p WHERE UserID = @id",
                new SqlParameter("@p", Security.Hash(newPassword)),
                new SqlParameter("@id", userId));
        }

        public void DeactivateUser(int id)
        {
            _db.ExecuteNonQuery(
                "UPDATE Users SET IsActive = 0 WHERE UserID = @id",
                new SqlParameter("@id", id));
        }

        private static User MapUser(DataRow r)
        {
            User user = User.Create(Convert.ToString(r["Role"]));
            user.UserID = Convert.ToInt32(r["UserID"]);
            user.Username = Convert.ToString(r["Username"]);
            user.FullName = Convert.ToString(r["FullName"]);
            user.Email = r["Email"] == DBNull.Value ? string.Empty : Convert.ToString(r["Email"]);
            user.Phone = r["Phone"] == DBNull.Value ? string.Empty : Convert.ToString(r["Phone"]);
            user.IsActive = Convert.ToBoolean(r["IsActive"]);
            return user;
        }
    }
}
