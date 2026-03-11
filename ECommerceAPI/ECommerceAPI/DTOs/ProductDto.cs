using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Photo is required")]
        public string PhotoUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "At least one category is required")]
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Photo is required")]
        public string PhotoUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "At least one category is required")]
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class ProductListDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int SoldCount { get; set; }
        public bool IsSoldOut => Quantity == 0;
        public List<string> Categories { get; set; } = new List<string>();
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int SoldCount { get; set; }
        public bool IsSoldOut => Quantity == 0;
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}