using System.ComponentModel.DataAnnotations;

namespace ProductMaintenance.Models
{
    // Renamed to avoid ambiguity with Entity.Product
    public class ProductSimple
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        
        public decimal Price { get; set; }
    }
}
