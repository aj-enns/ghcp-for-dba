using Microsoft.EntityFrameworkCore;
using RetailDb.Data;
using RetailDb.Models;

namespace RetailDb.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(RetailDbContext context)
    {
        if (context.Categories.Any())
        {
            Console.WriteLine("Database already contains data — skipping seed.");
            return;
        }

        Console.WriteLine("Seeding database...");

        // ── Categories ────────────────────────────────────────────
        var electronics = new Category { Name = "Electronics", Description = "Electronic devices and accessories" };
        var clothing = new Category { Name = "Clothing", Description = "Apparel and fashion items" };
        var homeGarden = new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies" };
        var sports = new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" };
        var beauty = new Category { Name = "Beauty & Personal Care", Description = "Beauty products and personal care items" };
        var food = new Category { Name = "Food & Beverage", Description = "Grocery and gourmet food items" };

        var subCategories = new List<Category>
        {
            new() { Name = "Smartphones", Description = "Mobile phones and accessories", ParentCategory = electronics },
            new() { Name = "Laptops", Description = "Notebook computers", ParentCategory = electronics },
            new() { Name = "Audio", Description = "Headphones, speakers, and audio gear", ParentCategory = electronics },
            new() { Name = "Men's Clothing", Description = "Shirts, pants, and accessories for men", ParentCategory = clothing },
            new() { Name = "Women's Clothing", Description = "Dresses, tops, and accessories for women", ParentCategory = clothing },
            new() { Name = "Footwear", Description = "Shoes, boots, and sandals", ParentCategory = clothing },
            new() { Name = "Furniture", Description = "Indoor and outdoor furniture", ParentCategory = homeGarden },
            new() { Name = "Kitchen", Description = "Cookware and kitchen appliances", ParentCategory = homeGarden },
            new() { Name = "Fitness Equipment", Description = "Exercise machines and accessories", ParentCategory = sports },
            new() { Name = "Skincare", Description = "Moisturizers, serums, and cleansers", ParentCategory = beauty },
        };

        var topCategories = new List<Category> { electronics, clothing, homeGarden, sports, beauty, food };
        await context.Categories.AddRangeAsync(topCategories);
        await context.Categories.AddRangeAsync(subCategories);
        await context.SaveChangesAsync();

        // ── Suppliers ─────────────────────────────────────────────
        var suppliers = new List<Supplier>
        {
            new() { CompanyName = "TechSource Inc.", ContactName = "Alice Morgan", ContactEmail = "alice@techsource.com", Phone = "555-100-1001", Address = "100 Silicon Ave", City = "San Jose", State = "CA", PostalCode = "95101", Country = "USA" },
            new() { CompanyName = "Fashion Forward LLC", ContactName = "Marcus Webb", ContactEmail = "marcus@fashionforward.com", Phone = "555-200-2002", Address = "200 Runway Blvd", City = "New York", State = "NY", PostalCode = "10001", Country = "USA" },
            new() { CompanyName = "HomeStyle Wholesale", ContactName = "Carla Diaz", ContactEmail = "carla@homestyle.com", Phone = "555-300-3003", Address = "300 Maple Dr", City = "Chicago", State = "IL", PostalCode = "60601", Country = "USA" },
            new() { CompanyName = "ActiveGear Co.", ContactName = "James Park", ContactEmail = "james@activegear.com", Phone = "555-400-4004", Address = "400 Fitness Pkwy", City = "Denver", State = "CO", PostalCode = "80201", Country = "USA" },
            new() { CompanyName = "Glow Essentials", ContactName = "Nina Torres", ContactEmail = "nina@glowessentials.com", Phone = "555-500-5005", Address = "500 Spa Lane", City = "Miami", State = "FL", PostalCode = "33101", Country = "USA" },
            new() { CompanyName = "Harvest Foods Group", ContactName = "Roberto Silva", ContactEmail = "roberto@harvestfoods.com", Phone = "555-600-6006", Address = "600 Farm Rd", City = "Austin", State = "TX", PostalCode = "78701", Country = "USA" },
        };
        await context.Suppliers.AddRangeAsync(suppliers);
        await context.SaveChangesAsync();

        // ── Stores ────────────────────────────────────────────────
        var stores = new List<Store>
        {
            new() { StoreName = "Downtown Flagship", Address = "1 Main Street", City = "New York", State = "NY", PostalCode = "10001", Phone = "212-555-0101", ManagerName = "Sarah Kim", OpenedAt = new DateTime(2018, 3, 15) },
            new() { StoreName = "Westside Mall", Address = "250 Westfield Blvd", City = "Los Angeles", State = "CA", PostalCode = "90001", Phone = "310-555-0202", ManagerName = "David Reyes", OpenedAt = new DateTime(2019, 7, 1) },
            new() { StoreName = "Northgate Center", Address = "500 North Ave", City = "Chicago", State = "IL", PostalCode = "60601", Phone = "312-555-0303", ManagerName = "Patricia Nguyen", OpenedAt = new DateTime(2020, 1, 20) },
            new() { StoreName = "Southpark Square", Address = "800 South Dr", City = "Houston", State = "TX", PostalCode = "77001", Phone = "713-555-0404", ManagerName = "Derek Johnson", OpenedAt = new DateTime(2021, 5, 10) },
            new() { StoreName = "Eastside Express", Address = "300 East Commerce", City = "Phoenix", State = "AZ", PostalCode = "85001", Phone = "602-555-0505", ManagerName = "Michelle Carter", OpenedAt = new DateTime(2022, 9, 5) },
        };
        await context.Stores.AddRangeAsync(stores);
        await context.SaveChangesAsync();

        // ── Employees ─────────────────────────────────────────────
        var employees = new List<Employee>
        {
            // Store 1 — Downtown Flagship
            new() { FirstName = "Sarah", LastName = "Kim", Email = "sarah.kim@retaillab.com", JobTitle = "Store Manager", Department = "Management", Salary = 75000, Store = stores[0], HireDate = new DateTime(2018, 3, 1) },
            new() { FirstName = "Tom", LastName = "Bradley", Email = "tom.bradley@retaillab.com", JobTitle = "Sales Associate", Department = "Sales", Salary = 38000, Store = stores[0], HireDate = new DateTime(2019, 6, 1) },
            new() { FirstName = "Ava", LastName = "Chen", Email = "ava.chen@retaillab.com", JobTitle = "Cashier", Department = "Operations", Salary = 32000, Store = stores[0], HireDate = new DateTime(2020, 2, 14) },
            // Store 2 — Westside Mall
            new() { FirstName = "David", LastName = "Reyes", Email = "david.reyes@retaillab.com", JobTitle = "Store Manager", Department = "Management", Salary = 72000, Store = stores[1], HireDate = new DateTime(2019, 7, 1) },
            new() { FirstName = "Lisa", LastName = "Patel", Email = "lisa.patel@retaillab.com", JobTitle = "Assistant Manager", Department = "Management", Salary = 52000, Store = stores[1], HireDate = new DateTime(2019, 8, 15) },
            new() { FirstName = "Kevin", LastName = "Walsh", Email = "kevin.walsh@retaillab.com", JobTitle = "Sales Associate", Department = "Sales", Salary = 36000, Store = stores[1], HireDate = new DateTime(2020, 11, 1) },
            // Store 3 — Northgate
            new() { FirstName = "Patricia", LastName = "Nguyen", Email = "patricia.nguyen@retaillab.com", JobTitle = "Store Manager", Department = "Management", Salary = 70000, Store = stores[2], HireDate = new DateTime(2020, 1, 20) },
            new() { FirstName = "Omar", LastName = "Hassan", Email = "omar.hassan@retaillab.com", JobTitle = "Inventory Specialist", Department = "Operations", Salary = 42000, Store = stores[2], HireDate = new DateTime(2020, 4, 1) },
            // Store 4 — Southpark Square
            new() { FirstName = "Derek", LastName = "Johnson", Email = "derek.johnson@retaillab.com", JobTitle = "Store Manager", Department = "Management", Salary = 68000, Store = stores[3], HireDate = new DateTime(2021, 5, 10) },
            new() { FirstName = "Angela", LastName = "Brooks", Email = "angela.brooks@retaillab.com", JobTitle = "Sales Associate", Department = "Sales", Salary = 35000, Store = stores[3], HireDate = new DateTime(2021, 7, 15) },
            // Store 5 — Eastside Express
            new() { FirstName = "Michelle", LastName = "Carter", Email = "michelle.carter@retaillab.com", JobTitle = "Store Manager", Department = "Management", Salary = 66000, Store = stores[4], HireDate = new DateTime(2022, 9, 5) },
            new() { FirstName = "Raj", LastName = "Mehta", Email = "raj.mehta@retaillab.com", JobTitle = "Cashier", Department = "Operations", Salary = 31000, Store = stores[4], HireDate = new DateTime(2022, 10, 1) },
        };
        // Set manager relationships for assistant managers / associates
        employees[1].Manager = employees[0];  // Tom -> Sarah (Store 1)
        employees[2].Manager = employees[0];  // Ava  -> Sarah (Store 1)
        employees[4].Manager = employees[3];  // Lisa -> David (Store 2)
        employees[5].Manager = employees[3];  // Kevin-> David (Store 2)
        employees[7].Manager = employees[6];  // Omar -> Patricia (Store 3)
        employees[9].Manager = employees[8];  // Angela -> Derek (Store 4)
        employees[11].Manager = employees[10]; // Raj -> Michelle (Store 5)

        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();

        // ── Products ──────────────────────────────────────────────
        var products = new List<Product>
        {
            // Electronics — Smartphones
            new() { Sku = "ELEC-SP-001", Name = "ProPhone X15", Description = "6.7\" OLED display, 256GB, latest flagship smartphone", Category = subCategories[0], Supplier = suppliers[0], UnitPrice = 999.99m, CostPrice = 620.00m, ReorderLevel = 5, ReorderQuantity = 20 },
            new() { Sku = "ELEC-SP-002", Name = "BudgetPhone 12", Description = "5.5\" LCD, 64GB, affordable everyday smartphone", Category = subCategories[0], Supplier = suppliers[0], UnitPrice = 299.99m, CostPrice = 160.00m, ReorderLevel = 10, ReorderQuantity = 50 },
            new() { Sku = "ELEC-SP-003", Name = "ProPhone X15 Case", Description = "Slim-fit protective case for ProPhone X15", Category = subCategories[0], Supplier = suppliers[0], UnitPrice = 29.99m, CostPrice = 8.00m, ReorderLevel = 20, ReorderQuantity = 100 },
            // Electronics — Laptops
            new() { Sku = "ELEC-LT-001", Name = "UltraBook Pro 14", Description = "14\" 4K touchscreen, Intel Core i7, 16GB RAM, 512GB SSD", Category = subCategories[1], Supplier = suppliers[0], UnitPrice = 1299.99m, CostPrice = 820.00m, ReorderLevel = 3, ReorderQuantity = 15 },
            new() { Sku = "ELEC-LT-002", Name = "WorkStation 17", Description = "17\" IPS, AMD Ryzen 9, 32GB RAM, 1TB SSD, dedicated GPU", Category = subCategories[1], Supplier = suppliers[0], UnitPrice = 1899.99m, CostPrice = 1200.00m, ReorderLevel = 2, ReorderQuantity = 10 },
            // Electronics — Audio
            new() { Sku = "ELEC-AU-001", Name = "SoundWave BT Headphones", Description = "Over-ear Bluetooth headphones, 30-hour battery, active noise cancellation", Category = subCategories[2], Supplier = suppliers[0], UnitPrice = 199.99m, CostPrice = 95.00m, ReorderLevel = 10, ReorderQuantity = 40 },
            new() { Sku = "ELEC-AU-002", Name = "TrueSound Earbuds Pro", Description = "True wireless earbuds, IPX5 waterproof, 8-hour playback", Category = subCategories[2], Supplier = suppliers[0], UnitPrice = 129.99m, CostPrice = 55.00m, ReorderLevel = 15, ReorderQuantity = 60 },
            // Clothing — Men's
            new() { Sku = "CLTH-MN-001", Name = "Classic Oxford Shirt", Description = "100% cotton button-down, available in 5 colors", Category = subCategories[3], Supplier = suppliers[1], UnitPrice = 49.99m, CostPrice = 18.00m, ReorderLevel = 20, ReorderQuantity = 100 },
            new() { Sku = "CLTH-MN-002", Name = "Slim Fit Chino Pants", Description = "Stretch cotton blend, multiple color options", Category = subCategories[3], Supplier = suppliers[1], UnitPrice = 59.99m, CostPrice = 22.00m, ReorderLevel = 15, ReorderQuantity = 80 },
            new() { Sku = "CLTH-MN-003", Name = "Athletic Performance Tee", Description = "Moisture-wicking polyester, lightweight and breathable", Category = subCategories[3], Supplier = suppliers[1], UnitPrice = 24.99m, CostPrice = 8.00m, ReorderLevel = 30, ReorderQuantity = 150 },
            // Clothing — Women's
            new() { Sku = "CLTH-WN-001", Name = "Floral Wrap Dress", Description = "Lightweight midi dress with adjustable tie waist", Category = subCategories[4], Supplier = suppliers[1], UnitPrice = 69.99m, CostPrice = 25.00m, ReorderLevel = 15, ReorderQuantity = 75 },
            new() { Sku = "CLTH-WN-002", Name = "High-Waist Yoga Leggings", Description = "4-way stretch fabric, hidden waistband pocket", Category = subCategories[4], Supplier = suppliers[1], UnitPrice = 54.99m, CostPrice = 20.00m, ReorderLevel = 20, ReorderQuantity = 100 },
            new() { Sku = "CLTH-WN-003", Name = "Cashmere Blend Sweater", Description = "70% cashmere, 30% merino wool, v-neck style", Category = subCategories[4], Supplier = suppliers[1], UnitPrice = 119.99m, CostPrice = 55.00m, ReorderLevel = 10, ReorderQuantity = 40 },
            // Footwear
            new() { Sku = "CLTH-FW-001", Name = "Running Shoe Pro X", Description = "Advanced cushioning, carbon fiber plate, neutral runner", Category = subCategories[5], Supplier = suppliers[1], UnitPrice = 149.99m, CostPrice = 72.00m, ReorderLevel = 10, ReorderQuantity = 50 },
            new() { Sku = "CLTH-FW-002", Name = "Leather Chelsea Boot", Description = "Full-grain leather, elastic side panels, stacked heel", Category = subCategories[5], Supplier = suppliers[1], UnitPrice = 179.99m, CostPrice = 85.00m, ReorderLevel = 8, ReorderQuantity = 40 },
            // Home — Kitchen
            new() { Sku = "HOME-KT-001", Name = "Non-Stick Cookware Set (10pc)", Description = "Hard-anodized aluminum, dishwasher safe, oven safe to 400°F", Category = subCategories[7], Supplier = suppliers[2], UnitPrice = 189.99m, CostPrice = 90.00m, ReorderLevel = 5, ReorderQuantity = 25 },
            new() { Sku = "HOME-KT-002", Name = "Espresso Machine Deluxe", Description = "15-bar pump, built-in milk frother, 1.8L water reservoir", Category = subCategories[7], Supplier = suppliers[2], UnitPrice = 299.99m, CostPrice = 155.00m, ReorderLevel = 3, ReorderQuantity = 15 },
            new() { Sku = "HOME-KT-003", Name = "Chef's Knife 8\"", Description = "High-carbon stainless steel, ergonomic pakkawood handle", Category = subCategories[7], Supplier = suppliers[2], UnitPrice = 79.99m, CostPrice = 30.00m, ReorderLevel = 10, ReorderQuantity = 40 },
            // Sports — Fitness
            new() { Sku = "SPRT-FT-001", Name = "Adjustable Dumbbell Set", Description = "5–52.5 lbs per dumbbell, dial-select mechanism", Category = subCategories[8], Supplier = suppliers[3], UnitPrice = 349.99m, CostPrice = 185.00m, ReorderLevel = 3, ReorderQuantity = 10 },
            new() { Sku = "SPRT-FT-002", Name = "Resistance Band Set (5pc)", Description = "Latex-free, 10–50 lb resistance levels with carry bag", Category = subCategories[8], Supplier = suppliers[3], UnitPrice = 39.99m, CostPrice = 12.00m, ReorderLevel = 20, ReorderQuantity = 100 },
            new() { Sku = "SPRT-FT-003", Name = "Yoga Mat Premium", Description = "6mm TPE foam, non-slip texture, includes carrying strap", Category = subCategories[8], Supplier = suppliers[3], UnitPrice = 49.99m, CostPrice = 18.00m, ReorderLevel = 15, ReorderQuantity = 60 },
            // Beauty — Skincare
            new() { Sku = "BEAU-SK-001", Name = "Daily Moisturizer SPF 30", Description = "Oil-free formula, hyaluronic acid, broad spectrum SPF 30, 2 oz", Category = subCategories[9], Supplier = suppliers[4], UnitPrice = 34.99m, CostPrice = 12.00m, ReorderLevel = 25, ReorderQuantity = 100 },
            new() { Sku = "BEAU-SK-002", Name = "Vitamin C Brightening Serum", Description = "15% L-ascorbic acid, ferulic acid, 1 fl oz dropper bottle", Category = subCategories[9], Supplier = suppliers[4], UnitPrice = 44.99m, CostPrice = 16.00m, ReorderLevel = 20, ReorderQuantity = 80 },
            new() { Sku = "BEAU-SK-003", Name = "Retinol Night Cream", Description = "0.1% encapsulated retinol, peptide complex, 1.7 oz jar", Category = subCategories[9], Supplier = suppliers[4], UnitPrice = 52.99m, CostPrice = 20.00m, ReorderLevel = 15, ReorderQuantity = 60 },
            // Food & Beverage
            new() { Sku = "FOOD-GB-001", Name = "Artisan Coffee Blend 12oz", Description = "Medium roast, single origin Ethiopia, whole bean", Category = food, Supplier = suppliers[5], UnitPrice = 18.99m, CostPrice = 8.00m, ReorderLevel = 30, ReorderQuantity = 150 },
            new() { Sku = "FOOD-GB-002", Name = "Organic Green Tea (50 bags)", Description = "Japanese sencha, individually wrapped, certified organic", Category = food, Supplier = suppliers[5], UnitPrice = 14.99m, CostPrice = 5.00m, ReorderLevel = 40, ReorderQuantity = 200 },
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // ── Customers ─────────────────────────────────────────────
        var customers = new List<Customer>
        {
            new() { FirstName = "Emma", LastName = "Wilson", Email = "emma.wilson@email.com", Phone = "555-111-0001", Address = "10 Oak St", City = "New York", State = "NY", PostalCode = "10001", DateOfBirth = new DateTime(1990, 4, 12), RegisteredAt = new DateTime(2021, 1, 15) },
            new() { FirstName = "Liam", LastName = "Garcia", Email = "liam.garcia@email.com", Phone = "555-111-0002", Address = "22 Elm Ave", City = "Los Angeles", State = "CA", PostalCode = "90001", DateOfBirth = new DateTime(1985, 8, 23), RegisteredAt = new DateTime(2021, 3, 8) },
            new() { FirstName = "Olivia", LastName = "Martinez", Email = "olivia.martinez@email.com", Phone = "555-111-0003", Address = "45 Pine Rd", City = "Chicago", State = "IL", PostalCode = "60601", DateOfBirth = new DateTime(1993, 11, 5), RegisteredAt = new DateTime(2021, 5, 22) },
            new() { FirstName = "Noah", LastName = "Davis", Email = "noah.davis@email.com", Phone = "555-111-0004", Address = "78 Cedar Blvd", City = "Houston", State = "TX", PostalCode = "77001", DateOfBirth = new DateTime(1988, 2, 17), RegisteredAt = new DateTime(2021, 7, 14) },
            new() { FirstName = "Sophia", LastName = "Brown", Email = "sophia.brown@email.com", Phone = "555-111-0005", Address = "99 Birch Ln", City = "Phoenix", State = "AZ", PostalCode = "85001", DateOfBirth = new DateTime(1995, 6, 30), RegisteredAt = new DateTime(2021, 9, 3) },
            new() { FirstName = "Jackson", LastName = "Taylor", Email = "jackson.taylor@email.com", Phone = "555-111-0006", Address = "200 Maple St", City = "Philadelphia", State = "PA", PostalCode = "19101", DateOfBirth = new DateTime(1982, 12, 8), RegisteredAt = new DateTime(2022, 1, 19) },
            new() { FirstName = "Ava", LastName = "Anderson", Email = "ava.anderson@email.com", Phone = "555-111-0007", Address = "315 Walnut Ave", City = "San Antonio", State = "TX", PostalCode = "78201", DateOfBirth = new DateTime(1997, 3, 25), RegisteredAt = new DateTime(2022, 3, 7) },
            new() { FirstName = "Lucas", LastName = "Thomas", Email = "lucas.thomas@email.com", Phone = "555-111-0008", Address = "420 Hickory Dr", City = "San Diego", State = "CA", PostalCode = "92101", DateOfBirth = new DateTime(1991, 9, 14), RegisteredAt = new DateTime(2022, 6, 11) },
            new() { FirstName = "Mia", LastName = "White", Email = "mia.white@email.com", Phone = "555-111-0009", Address = "550 Willow Ct", City = "Dallas", State = "TX", PostalCode = "75201", DateOfBirth = new DateTime(1994, 7, 2), RegisteredAt = new DateTime(2022, 8, 25) },
            new() { FirstName = "Ethan", LastName = "Harris", Email = "ethan.harris@email.com", Phone = "555-111-0010", Address = "660 Spruce Way", City = "San Jose", State = "CA", PostalCode = "95101", DateOfBirth = new DateTime(1987, 5, 19), RegisteredAt = new DateTime(2022, 10, 30) },
            new() { FirstName = "Amelia", LastName = "Jackson", Email = "amelia.jackson@email.com", Phone = "555-111-0011", Address = "720 Aspen Pl", City = "Austin", State = "TX", PostalCode = "78701", DateOfBirth = new DateTime(1999, 1, 8), RegisteredAt = new DateTime(2023, 2, 14) },
            new() { FirstName = "Benjamin", LastName = "Lee", Email = "benjamin.lee@email.com", Phone = "555-111-0012", Address = "830 Chestnut Rd", City = "Jacksonville", State = "FL", PostalCode = "32201", DateOfBirth = new DateTime(1986, 10, 31), RegisteredAt = new DateTime(2023, 4, 5) },
            new() { FirstName = "Charlotte", LastName = "Robinson", Email = "charlotte.robinson@email.com", Phone = "555-111-0013", Address = "940 Poplar St", City = "Fort Worth", State = "TX", PostalCode = "76101", DateOfBirth = new DateTime(1992, 4, 22), RegisteredAt = new DateTime(2023, 6, 18) },
            new() { FirstName = "Henry", LastName = "Clark", Email = "henry.clark@email.com", Phone = "555-111-0014", Address = "1050 Sycamore Blvd", City = "Columbus", State = "OH", PostalCode = "43201", DateOfBirth = new DateTime(1983, 8, 11), RegisteredAt = new DateTime(2023, 8, 29) },
            new() { FirstName = "Abigail", LastName = "Lewis", Email = "abigail.lewis@email.com", Phone = "555-111-0015", Address = "1160 Dogwood Ave", City = "Charlotte", State = "NC", PostalCode = "28201", DateOfBirth = new DateTime(1996, 12, 27), RegisteredAt = new DateTime(2024, 1, 10) },
        };
        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        // ── Orders & OrderItems ───────────────────────────────────
        var rand = new Random(42);
        var orderStatuses = new[] { "Delivered", "Delivered", "Delivered", "Shipped", "Processing", "Pending" };
        var orders = new List<Order>();

        // Helper: create a realistic order
        Order MakeOrder(Customer customer, Store store, Employee employee, DateTime orderDate, string status, params (Product product, int qty, decimal discount)[] lines)
        {
            var items = lines.Select(l =>
            {
                var lineTotal = l.product.UnitPrice * l.qty * (1 - l.discount / 100);
                return new OrderItem
                {
                    Product = l.product,
                    Quantity = l.qty,
                    UnitPrice = l.product.UnitPrice,
                    DiscountPercent = l.discount,
                    LineTotal = Math.Round(lineTotal, 2)
                };
            }).ToList();

            var subtotal = items.Sum(i => i.LineTotal);
            var tax = Math.Round(subtotal * 0.08m, 2);
            var shipping = subtotal >= 100 ? 0m : 9.99m;
            var discount = 0m;
            var total = subtotal + tax + shipping - discount;

            return new Order
            {
                Customer = customer,
                Store = store,
                Employee = employee,
                OrderDate = orderDate,
                Status = status,
                ShipAddress = customer.Address,
                ShipCity = customer.City,
                ShipState = customer.State,
                ShipPostalCode = customer.PostalCode,
                ShipCountry = customer.Country ?? "USA",
                Subtotal = subtotal,
                TaxAmount = tax,
                ShippingAmount = shipping,
                DiscountAmount = discount,
                TotalAmount = total,
                ShippedDate = status is "Shipped" or "Delivered" ? orderDate.AddDays(2) : null,
                DeliveredDate = status == "Delivered" ? orderDate.AddDays(5) : null,
                OrderItems = items
            };
        }

        orders.AddRange(new[]
        {
            MakeOrder(customers[0], stores[0], employees[1], new DateTime(2024, 1, 10), "Delivered",
                (products[0], 1, 0), (products[2], 1, 0)),
            MakeOrder(customers[1], stores[1], employees[5], new DateTime(2024, 1, 22), "Delivered",
                (products[3], 1, 5)),
            MakeOrder(customers[2], stores[2], employees[7], new DateTime(2024, 2, 5), "Delivered",
                (products[7], 2, 0), (products[8], 1, 10)),
            MakeOrder(customers[3], stores[3], employees[9], new DateTime(2024, 2, 18), "Delivered",
                (products[18], 1, 0), (products[19], 2, 0)),
            MakeOrder(customers[4], stores[4], employees[11], new DateTime(2024, 3, 3), "Delivered",
                (products[21], 1, 0), (products[22], 1, 0)),
            MakeOrder(customers[5], stores[0], employees[1], new DateTime(2024, 3, 15), "Delivered",
                (products[15], 1, 0), (products[16], 1, 0)),
            MakeOrder(customers[6], stores[1], employees[5], new DateTime(2024, 4, 2), "Delivered",
                (products[5], 1, 0), (products[6], 1, 15)),
            MakeOrder(customers[7], stores[2], employees[7], new DateTime(2024, 4, 20), "Delivered",
                (products[10], 1, 0), (products[13], 1, 0)),
            MakeOrder(customers[8], stores[3], employees[9], new DateTime(2024, 5, 8), "Delivered",
                (products[24], 2, 0), (products[25], 3, 0)),
            MakeOrder(customers[9], stores[4], employees[11], new DateTime(2024, 5, 25), "Delivered",
                (products[4], 1, 0)),
            MakeOrder(customers[10], stores[0], employees[2], new DateTime(2024, 6, 10), "Delivered",
                (products[11], 2, 0), (products[20], 1, 0)),
            MakeOrder(customers[11], stores[1], employees[5], new DateTime(2024, 6, 28), "Delivered",
                (products[23], 1, 0), (products[1], 1, 10)),
            MakeOrder(customers[12], stores[2], employees[6], new DateTime(2024, 7, 15), "Delivered",
                (products[9], 3, 0), (products[8], 2, 5)),
            MakeOrder(customers[13], stores[3], employees[8], new DateTime(2024, 8, 2), "Delivered",
                (products[17], 1, 0)),
            MakeOrder(customers[14], stores[4], employees[10], new DateTime(2024, 8, 20), "Delivered",
                (products[12], 1, 0), (products[14], 1, 0)),
            // More recent orders with mixed statuses
            MakeOrder(customers[0], stores[0], employees[1], new DateTime(2024, 9, 5), "Delivered",
                (products[5], 1, 10), (products[25], 2, 0)),
            MakeOrder(customers[2], stores[2], employees[7], new DateTime(2024, 9, 18), "Delivered",
                (products[0], 1, 0)),
            MakeOrder(customers[4], stores[1], employees[4], new DateTime(2024, 10, 1), "Shipped",
                (products[19], 3, 0), (products[20], 1, 0)),
            MakeOrder(customers[6], stores[3], employees[9], new DateTime(2024, 10, 14), "Shipped",
                (products[3], 1, 8)),
            MakeOrder(customers[8], stores[0], employees[1], new DateTime(2024, 10, 27), "Processing",
                (products[21], 2, 0), (products[22], 1, 5)),
            MakeOrder(customers[1], stores[4], employees[11], new DateTime(2024, 11, 5), "Pending",
                (products[15], 1, 0), (products[17], 1, 0)),
        });

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        // ── Inventory ─────────────────────────────────────────────
        var inventories = new List<Inventory>();
        foreach (var store in stores)
        {
            foreach (var product in products)
            {
                inventories.Add(new Inventory
                {
                    Product = product,
                    Store = store,
                    QuantityOnHand = rand.Next(5, 150),
                    QuantityReserved = rand.Next(0, 10),
                    LastCountDate = DateTime.UtcNow.AddDays(-rand.Next(1, 60)),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        await context.Inventories.AddRangeAsync(inventories);
        await context.SaveChangesAsync();

        // ── Reviews ───────────────────────────────────────────────
        var reviews = new List<Review>
        {
            new() { Product = products[0], Customer = customers[0], Rating = 5, Title = "Best phone I've ever owned", Body = "The camera is incredible and the battery lasts all day. Highly recommend.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 1, 20) },
            new() { Product = products[0], Customer = customers[2], Rating = 4, Title = "Great phone, expensive", Body = "Performs flawlessly but the price point is steep. Worth it if you need the best.", IsVerifiedPurchase = false, CreatedAt = new DateTime(2024, 2, 14) },
            new() { Product = products[3], Customer = customers[1], Rating = 5, Title = "Laptop replaced my desktop", Body = "Blazing fast performance, stunning display. I never miss my old desktop.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 2, 5) },
            new() { Product = products[5], Customer = customers[6], Rating = 5, Title = "Noise cancellation is a game-changer", Body = "Using these on my commute every day. The ANC blocks out everything.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 4, 15) },
            new() { Product = products[7], Customer = customers[2], Rating = 4, Title = "Classic, well-made shirt", Body = "Good fit right off the rack. Material is soft and breathable.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 2, 20) },
            new() { Product = products[10], Customer = customers[7], Rating = 5, Title = "Absolutely love this dress", Body = "Comfortable, flattering and the pattern is gorgeous. Already ordered another color.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 5, 2) },
            new() { Product = products[18], Customer = customers[3], Rating = 5, Title = "Changed my home gym setup", Body = "Compact, easy to adjust, and they feel solid. Worth every penny.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 3, 1) },
            new() { Product = products[21], Customer = customers[4], Rating = 4, Title = "Great moisturizer", Body = "Lightweight, absorbs fast, no greasy feeling. Good SPF too.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 3, 18) },
            new() { Product = products[24], Customer = customers[8], Rating = 5, Title = "Coffee snob approved", Body = "Best coffee I've found outside a specialty café. Rich, smooth, no bitterness.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 5, 20) },
            new() { Product = products[15], Customer = customers[5], Rating = 3, Title = "Good set, tricky cleanup", Body = "Non-stick works well initially but requires careful hand washing. Not as dishwasher-safe as advertised.", IsVerifiedPurchase = true, CreatedAt = new DateTime(2024, 3, 28) },
        };
        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Database seeded successfully.");
        Console.WriteLine($"   Categories : {await context.Categories.CountAsync()}");
        Console.WriteLine($"   Suppliers  : {await context.Suppliers.CountAsync()}");
        Console.WriteLine($"   Products   : {await context.Products.CountAsync()}");
        Console.WriteLine($"   Customers  : {await context.Customers.CountAsync()}");
        Console.WriteLine($"   Stores     : {await context.Stores.CountAsync()}");
        Console.WriteLine($"   Employees  : {await context.Employees.CountAsync()}");
        Console.WriteLine($"   Orders     : {await context.Orders.CountAsync()}");
        Console.WriteLine($"   OrderItems : {await context.OrderItems.CountAsync()}");
        Console.WriteLine($"   Inventory  : {await context.Inventories.CountAsync()}");
        Console.WriteLine($"   Reviews    : {await context.Reviews.CountAsync()}");
    }
}
