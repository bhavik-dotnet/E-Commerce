using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class ProductCategory
    {
        [Key]
        public int ProductCategoryId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
}