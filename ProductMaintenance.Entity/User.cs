using System.ComponentModel.DataAnnotations;

namespace ProductMaintenance.Entity
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int UserTypeId { get; set; }

        public UserType UserType { get; set; } = null!;

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
