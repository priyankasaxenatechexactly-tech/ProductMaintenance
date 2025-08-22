using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ProductMaintenance.Models
{
    public class UserType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
