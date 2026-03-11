using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public interface IRatingRepository
    {
        Task<List<ProductRating>> GetProductRatingsAsync(int productId);
        Task<ProductRating?> GetUserProductRatingAsync(int userId, int productId);
        Task<ProductRating> CreateRatingAsync(ProductRating rating);
        Task<ProductRating> UpdateRatingAsync(ProductRating rating);
    }

    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductRating>> GetProductRatingsAsync(int productId)
        {
            return await _context.ProductRatings
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<ProductRating?> GetUserProductRatingAsync(int userId, int productId)
        {
            return await _context.ProductRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<ProductRating> CreateRatingAsync(ProductRating rating)
        {
            _context.ProductRatings.Add(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task<ProductRating> UpdateRatingAsync(ProductRating rating)
        {
            _context.ProductRatings.Update(rating);
            await _context.SaveChangesAsync();
            return rating;
        }
    }
}