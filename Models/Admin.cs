namespace GameHub.Models
{
    public class Admin : User
    {
        public override string Role { get { return "Admin"; } }
        public override string GetHomeForm() { return "Dashboard"; }
    }
}
