namespace ProductMaintenance.Models
{
    public class UserListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Full name: Name + LastName
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
    }
}
