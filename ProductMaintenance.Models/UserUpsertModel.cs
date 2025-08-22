using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductMaintenance.Models
{
    public class UserUpsertModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [RegularExpression(@"^\d*$", ErrorMessage = "Mobile number must be digits only.")]
        public string? MobileNo { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int UserTypeId { get; set; }

        // For create/change password
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string? Password { get; set; }

        // Dropdown roles
        public List<RoleItem> Roles { get; set; } = new();

        // For messages
        public string? Error { get; set; }
        public string? Message { get; set; }
    }
}
