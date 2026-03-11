using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryDto>();

            // Product mappings
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.ProductCategories.Select(pc => pc.Category.CategoryName).ToList()))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.ProductRatings.Any() ? src.ProductRatings.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src =>
                    src.ProductRatings.Count));

            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.ProductCategories.Select(pc => new CategoryDto
                    {
                        CategoryId = pc.Category.CategoryId,
                        CategoryName = pc.Category.CategoryName
                    }).ToList()))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.ProductRatings.Any() ? src.ProductRatings.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src =>
                    src.ProductRatings.Count));

            // Cart mappings
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Product.PhotoUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Product.Price * src.Quantity))
                .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Product.Quantity));

            // Order mappings
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Product.PhotoUrl));

            // Rating mappings
            CreateMap<CreateRatingDto, ProductRating>();
            CreateMap<ProductRating, RatingDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

            // User mappings
            CreateMap<User, LoginResponseDto>();
        }
    }
}