using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    // Product service contract
    public interface IProductService
    {
        Task<List<ProductListDto>> GetAllProductsAsync();
        Task<ProductDetailDto?> GetProductByIdAsync(int id);
        Task<ProductDetailDto> CreateProductAsync(CreateProductDto productDto);
        Task<ProductDetailDto?> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<PagedProductResponse> GetFilteredProductsAsync(ProductFilterDto filter);
    }

    // Handles product business logic
    public class ProductService : IProductService
    {
        // Product data access
        private readonly IProductRepository _productRepository;
        
        // Database context
        private readonly ApplicationDbContext _context;
        
        // Object mapper
        private readonly IMapper _mapper;

        // Initializes product service with dependencies
        public ProductService(
            IProductRepository productRepository,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _context = context;
            _mapper = mapper;
        }

        // Gets all products (basic info)
        public async Task<List<ProductListDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<List<ProductListDto>>(products);
        }

        // Gets product by ID with details
        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            return _mapper.Map<ProductDetailDto>(product);
        }

        // Creates new product with categories
        public async Task<ProductDetailDto> CreateProductAsync(CreateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var createdProduct = await _productRepository.CreateProductAsync(product);

            // Add category associations
            foreach (var categoryId in productDto.CategoryIds)
            {
                var productCategory = new ProductCategory
                {
                    ProductId = createdProduct.ProductId,
                    CategoryId = categoryId
                };
                _context.ProductCategories.Add(productCategory);
            }
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(createdProduct.ProductId)
                   ?? throw new Exception("Failed to retrieve created product");
        }

        // Updates existing product and categories
        public async Task<ProductDetailDto?> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null) return null;

            _mapper.Map(productDto, existingProduct);

            // Update category associations
            var existingCategories = _context.ProductCategories
                .Where(pc => pc.ProductId == id);
            _context.ProductCategories.RemoveRange(existingCategories);

            foreach (var categoryId in productDto.CategoryIds)
            {
                var productCategory = new ProductCategory
                {
                    ProductId = id,
                    CategoryId = categoryId
                };
                _context.ProductCategories.Add(productCategory);
            }

            await _productRepository.UpdateProductAsync(existingProduct);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(id);
        }

        // Deletes product by ID
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        // Gets all categories
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _productRepository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        // Gets filtered products with pagination
        public async Task<PagedProductResponse> GetFilteredProductsAsync(ProductFilterDto filter)
        {
            var (products, totalCount) = await _productRepository.GetFilteredProductsAsync(filter);
            var productDtos = _mapper.Map<List<ProductListDto>>(products);

            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            return new PagedProductResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasPreviousPage = filter.PageNumber > 1,
                HasNextPage = filter.PageNumber < totalPages
            };
        }
    }
}