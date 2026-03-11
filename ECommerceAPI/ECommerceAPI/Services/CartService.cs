using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<CartDto?> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto updateDto);
        Task<bool> RemoveFromCartAsync(int userId, int cartItemId);
        Task<bool> ClearCartAsync(int userId);
    }

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            var cartItemDtos = _mapper.Map<List<CartItemDto>>(cartItems);

            return new CartDto
            {
                Items = cartItemDtos,
                TotalAmount = cartItemDtos.Sum(i => i.Subtotal),
                TotalItems = cartItemDtos.Sum(i => i.Quantity)
            };
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            var product = await _productRepository.GetProductByIdAsync(addToCartDto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            if (product.Quantity < addToCartDto.Quantity)
                throw new Exception("Insufficient stock");

            var existingCartItem = await _cartRepository
                .GetCartItemByProductAsync(userId, addToCartDto.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += addToCartDto.Quantity;

                if (existingCartItem.Quantity > product.Quantity)
                    throw new Exception("Insufficient stock");

                await _cartRepository.UpdateCartItemAsync(existingCartItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            return await GetCartAsync(userId);
        }

        public async Task<CartDto?> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto updateDto)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(cartItemId, userId);
            if (cartItem == null) return null;

            var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
            if (product == null || product.Quantity < updateDto.Quantity)
                throw new Exception("Insufficient stock");

            cartItem.Quantity = updateDto.Quantity;
            await _cartRepository.UpdateCartItemAsync(cartItem);

            return await GetCartAsync(userId);
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
        {
            return await _cartRepository.RemoveCartItemAsync(cartItemId, userId);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            return await _cartRepository.ClearCartAsync(userId);
        }
    }
}