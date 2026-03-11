using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CreateRatingDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(500, ErrorMessage = "Review cannot exceed 500 characters")]
        public string? Review { get; set; }
    }

    public class RatingDto
    {
        public int RatingId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}