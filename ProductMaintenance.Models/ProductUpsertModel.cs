using System.ComponentModel.DataAnnotations;

namespace ProductMaintenance.Models
{
    public class ProductUpsertModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, 999999999.99)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public List<string> Categories { get; set; } = new List<string>
        {
            "Electronics","Clothing","Groceries","Books","Furniture","Sports"
        };
    }
}
