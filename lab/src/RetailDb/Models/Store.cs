namespace RetailDb.Models;

public class Store
{
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "USA";
    public string? Phone { get; set; }
    public string? ManagerName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime OpenedAt { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
