namespace ECommerceAPI.DTOs
{
    public class ProductFilterDto
    {
        public string? SearchTerm { get; set; }
        public List<int>? CategoryIds { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; } // "name", "price", "rating", "sold"
        public string? SortOrder { get; set; } // "asc", "desc"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class PagedProductResponse
    {
        public List<ProductListDto> Products { get; set; } = new List<ProductListDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}