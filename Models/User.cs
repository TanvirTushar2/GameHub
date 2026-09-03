namespace GameHub.Models
{
    /// <summary>
    /// Abstract base class for every user (Abstraction + Inheritance).
    /// Fields are private and exposed through public properties (Encapsulation).
    /// </summary>
    public abstract class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        /// <summary>Each role returns its own name (overridden - Polymorphism).</summary>
        public abstract string Role { get; }

        /// <summary>Role-specific landing form name (overridden - Polymorphism).</summary>
        public abstract string GetHomeForm();

        /// <summary>Factory: build the right subclass from a role string.</summary>
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
