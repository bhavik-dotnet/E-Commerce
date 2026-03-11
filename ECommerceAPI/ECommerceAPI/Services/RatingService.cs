using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public interface IRatingService
    {
        Task<List<RatingDto>> GetProductRatingsAsync(int productId);
        Task<RatingDto> CreateOrUpdateRatingAsync(int userId, CreateRatingDto ratingDto);
    }

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public RatingService(
            IRatingRepository ratingRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<RatingDto>> GetProductRatingsAsync(int productId)
        {
            var ratings = await _ratingRepository.GetProductRatingsAsync(productId);
            return _mapper.Map<List<RatingDto>>(ratings);
        }

        public async Task<RatingDto> CreateOrUpdateRatingAsync(int userId, CreateRatingDto ratingDto)
        {
            var product = await _productRepository.GetProductByIdAsync(ratingDto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            var existingRating = await _ratingRepository.GetUserProductRatingAsync(userId, ratingDto.ProductId);

            ProductRating rating;
            if (existingRating != null)
            {
                existingRating.Rating = ratingDto.Rating;
                existingRating.Review = ratingDto.Review;
                rating = await _ratingRepository.UpdateRatingAsync(existingRating);
            }
            else
            {
                rating = new ProductRating
                {
                    UserId = userId,
                    ProductId = ratingDto.ProductId,
                    Rating = ratingDto.Rating,
                    Review = ratingDto.Review
                };
                rating = await _ratingRepository.CreateRatingAsync(rating);
            }

            var ratings = await _ratingRepository.GetProductRatingsAsync(ratingDto.ProductId);
            var ratingDto2 = ratings.FirstOrDefault(r => r.RatingId == rating.RatingId);
            return _mapper.Map<RatingDto>(ratingDto2);
        }
    }
}