namespace GameHub.Models
{
    /// <summary>
    /// The owner of the whole GameHub system. A Super Admin can do everything an
    /// Admin can AND is the only role allowed to create, edit or deactivate other
    /// Admin (and Super Admin) accounts.
    /// </summary>
    public class SuperAdmin : User
    {
        public override string Role { get { return "SuperAdmin"; } }
        public override string GetHomeForm() { return "Dashboard"; }
    }
}
