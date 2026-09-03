namespace GameHub.Models
{
    public class Staff : User
    {
        public override string Role { get { return "Staff"; } }
        public override string GetHomeForm() { return "Dashboard"; }
    }
}
