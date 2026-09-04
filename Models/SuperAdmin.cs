namespace GameHub.Models
{

    public class SuperAdmin : User
    {
        public override string Role { get { return "SuperAdmin"; } }
        public override string GetHomeForm() { return "Dashboard"; }
    }
}
