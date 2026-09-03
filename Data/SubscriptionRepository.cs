using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GameHub.Models;

namespace GameHub.Data
{
    public class SubscriptionRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public List<SubscriptionPlan> GetPlans()
        {
            DataTable dt = _db.ExecuteQuery(
                "SELECT PlanID, PlanName, MonthlyPrice, DiscountPct, FreeGamesPerMonth FROM SubscriptionPlans ORDER BY MonthlyPrice");

            List<SubscriptionPlan> list = new List<SubscriptionPlan>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new SubscriptionPlan
                {
                    PlanID = Convert.ToInt32(r["PlanID"]),
                    PlanName = Convert.ToString(r["PlanName"]),
                    MonthlyPrice = Convert.ToDecimal(r["MonthlyPrice"]),
                    DiscountPct = Convert.ToInt32(r["DiscountPct"]),
                    FreeGamesPerMonth = Convert.ToInt32(r["FreeGamesPerMonth"])
                });
            }
            return list;
        }

        public int GetActiveDiscount(int userId)
        {
            object o = _db.ExecuteScalar(
                @"SELECT TOP 1 sp.DiscountPct
                  FROM Subscriptions s
                  JOIN SubscriptionPlans sp ON s.PlanID = sp.PlanID
                  WHERE s.UserID = @u
                    AND s.Status = 'Active'
                    AND s.EndDate >= CAST(GETDATE() AS DATE)
                  ORDER BY sp.DiscountPct DESC",
                new SqlParameter("@u", userId));
            return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
        }

        public string GetActivePlanName(int userId)
        {
            object o = _db.ExecuteScalar(
                @"SELECT TOP 1 sp.PlanName
                  FROM Subscriptions s
                  JOIN SubscriptionPlans sp ON s.PlanID = sp.PlanID
                  WHERE s.UserID = @u
                    AND s.Status = 'Active'
                    AND s.EndDate >= CAST(GETDATE() AS DATE)
                  ORDER BY sp.MonthlyPrice DESC",
                new SqlParameter("@u", userId));
            return o == null || o == DBNull.Value ? "None" : Convert.ToString(o);
        }

        public DateTime? GetActivePlanEndDate(int userId)
        {
            object o = _db.ExecuteScalar(
                @"SELECT TOP 1 s.EndDate
                  FROM Subscriptions s
                  JOIN SubscriptionPlans sp ON s.PlanID = sp.PlanID
                  WHERE s.UserID = @u
                    AND s.Status = 'Active'
                    AND s.EndDate >= CAST(GETDATE() AS DATE)
                  ORDER BY sp.MonthlyPrice DESC",
                new SqlParameter("@u", userId));
            return o == null || o == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(o);
        }

        /// <summary>
        /// Replace the active subscription and charge the wallet in one SQL transaction.
        /// The wallet transaction is recorded as a negative Spend amount.
        /// </summary>
        public void Subscribe(int userId, SubscriptionPlan plan)
        {
            if (plan == null) throw new ArgumentNullException("plan");

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

                        if (balance < plan.MonthlyPrice)
                            throw new InvalidOperationException("Insufficient wallet balance for this subscription.");

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Subscriptions SET Status = 'Cancelled' WHERE UserID = @u AND Status = 'Active'", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO Subscriptions (UserID, PlanID, StartDate, EndDate, Status)
                              VALUES (@u, @p, CAST(GETDATE() AS DATE),
                                      DATEADD(MONTH, 1, CAST(GETDATE() AS DATE)), 'Active')", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.Parameters.AddWithValue("@p", plan.PlanID);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Wallet SET Balance = Balance - @price WHERE UserID = @u", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@price", plan.MonthlyPrice);
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO WalletTransactions (WalletID, TxType, Amount)
                              SELECT WalletID, 'Subscription', -@price FROM Wallet WHERE UserID = @u", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@price", plan.MonthlyPrice);
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
    }
}
