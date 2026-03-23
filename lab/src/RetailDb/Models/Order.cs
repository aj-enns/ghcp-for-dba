namespace RetailDb.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int StoreId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipState { get; set; }
    public string? ShipPostalCode { get; set; }
    public string ShipCountry { get; set; } = "USA";
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public Customer Customer { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public Employee? Employee { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
