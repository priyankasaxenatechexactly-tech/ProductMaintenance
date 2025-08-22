namespace ProductMaintenance.Models
{
    public class UserItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
