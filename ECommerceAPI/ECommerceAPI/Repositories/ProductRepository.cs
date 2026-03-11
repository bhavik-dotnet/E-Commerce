using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(ProductFilterDto filter);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductRatings)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.SoldCount)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductRatings)
                .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductRatings)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Search by product name
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(filter.SearchTerm));
            }

            // Filter by categories
            if (filter.CategoryIds != null && filter.CategoryIds.Any())
            {
                query = query.Where(p => p.ProductCategories
                    .Any(pc => filter.CategoryIds.Contains(pc.CategoryId)));
            }

            // Filter by price range
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            // Filter by stock availability
            if (filter.InStock.HasValue)
            {
                if (filter.InStock.Value)
                {
                    query = query.Where(p => p.Quantity > 0);
                }
                else
                {
                    query = query.Where(p => p.Quantity == 0);
                }
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.ProductName)
                    : query.OrderBy(p => p.ProductName),
                "price" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "rating" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.ProductRatings.Any() ? p.ProductRatings.Average(r => r.Rating) : 0)
                    : query.OrderBy(p => p.ProductRatings.Any() ? p.ProductRatings.Average(r => r.Rating) : 0),
                "sold" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.SoldCount)
                    : query.OrderBy(p => p.SoldCount),
                _ => query.OrderByDescending(p => p.SoldCount) // Default: most sold first
            };

            // Pagination
            var products = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}