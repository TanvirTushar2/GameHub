using System.Data.SqlClient;

namespace GameHub.Data
{
    /// <summary>
    /// Reads and writes owner-level application settings (AppSettings table).
    /// Only the Super Admin "Settings" screen uses this.
    /// </summary>
    public class SettingsRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public string Get(string key, string fallback)
        {
            object o = _db.ExecuteScalar(
                "SELECT SettingValue FROM AppSettings WHERE SettingKey = @k",
                new SqlParameter("@k", key));
            return o == null || o == System.DBNull.Value ? fallback : o.ToString();
        }

        /// <summary>Insert or update a setting (upsert).</summary>
        public void Set(string key, string value)
        {
            _db.ExecuteNonQuery(
                @"IF EXISTS (SELECT 1 FROM AppSettings WHERE SettingKey = @k)
                      UPDATE AppSettings SET SettingValue = @v WHERE SettingKey = @k;
                  ELSE
                      INSERT INTO AppSettings (SettingKey, SettingValue) VALUES (@k, @v);",
                new SqlParameter("@k", key),
                new SqlParameter("@v", (object)(value ?? "")));
        }
    }
}
