namespace OrderManagementSystem.Entity;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public List<OrderItem> OrderItems { get; set; } = [];
    public Customer Customer { get; set; } = default!;
    public Invoice Invoice { get; set; } = default!;
}