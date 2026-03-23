namespace RetailDb.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public int ReorderLevel { get; set; } = 10;
    public int ReorderQuantity { get; set; } = 50;
    public bool IsDiscontinued { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
