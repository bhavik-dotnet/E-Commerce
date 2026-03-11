namespace ECommerceAPI.DTOs
{
    public class OrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal => Quantity * Price;
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderDto
    {
        public List<int> ProductIds { get; set; } = new();
        public List<int> Quantities { get; set; } = new();
    }
}
