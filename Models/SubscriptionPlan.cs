namespace GameHub.Models
{
    public class SubscriptionPlan
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int DiscountPct { get; set; }
        public int FreeGamesPerMonth { get; set; }
    }
}
