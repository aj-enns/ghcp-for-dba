namespace RetailDb.Models;

public class Inventory
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public DateTime LastCountDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
}
