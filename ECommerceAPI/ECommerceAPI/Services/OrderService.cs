using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerceAPI.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrderHistoryAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId);
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<OrderDto>> GetOrderHistoryAsync(int userId)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.ProductName,
                        PhotoUrl = oi.Product.PhotoUrl,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _dbContext.Orders
                .Where(o => o.UserId == userId && o.OrderId == orderId)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.ProductName,
                        PhotoUrl = oi.Product.PhotoUrl,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto)
        {
            if (createOrderDto.ProductIds.Count != createOrderDto.Quantities.Count)
                throw new Exception("ProductIds and Quantities count mismatch");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Pending",
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            for (int i = 0; i < createOrderDto.ProductIds.Count; i++)
            {
                var product = await _dbContext.Products.FindAsync(createOrderDto.ProductIds[i]);
                if (product == null) throw new Exception("Product not found");

                var quantity = createOrderDto.Quantities[i];

                var orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    Price = product.Price
                };

                order.TotalAmount += product.Price * quantity;
                order.OrderItems.Add(orderItem);
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product.ProductName,
                    PhotoUrl = oi.Product.PhotoUrl,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }
    }
}