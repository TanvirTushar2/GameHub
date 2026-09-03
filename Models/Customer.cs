namespace GameHub.Models
{
    public class Customer : User
    {
        public override string Role { get { return "Customer"; } }
        public override string GetHomeForm() { return "CustomerDashboard"; }
    }
}
