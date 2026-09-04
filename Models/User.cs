namespace GameHub.Models
{
   
    public abstract class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public abstract string Role { get; }

        public abstract string GetHomeForm();

        public static User Create(string role)
        {
            switch ((role ?? "").ToLower())
            {
                case "superadmin": return new SuperAdmin();
                case "admin":    return new Admin();
                case "staff":    return new Staff();
                default:          return new Customer();
            }
        }
    }
}
