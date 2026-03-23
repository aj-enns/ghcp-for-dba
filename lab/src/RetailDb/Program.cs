using Microsoft.EntityFrameworkCore;
using RetailDb.Data;
using RetailDb.Seed;

var connectionString = Environment.GetEnvironmentVariable("RETAILDB_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "RETAILDB_CONNECTION_STRING environment variable is not set.\n" +
        "Example: Server=tcp:<server>.database.windows.net,1433;Initial Catalog=RetailDb;" +
        "Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

var optionsBuilder = new DbContextOptionsBuilder<RetailDbContext>();
optionsBuilder.UseSqlServer(connectionString);

await using var context = new RetailDbContext(optionsBuilder.Options);

var command = args.FirstOrDefault()?.ToLowerInvariant() ?? "migrate";

switch (command)
{
    case "migrate":
        Console.WriteLine("Applying EF Core migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied.");
        break;

    case "seed":
        Console.WriteLine("Seeding retail data...");
        await DatabaseSeeder.SeedAsync(context);
        break;

    case "setup":
        Console.WriteLine("Running full setup (migrate + seed)...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied.");
        await DatabaseSeeder.SeedAsync(context);
        break;

    case "drop":
        Console.WriteLine("⚠️  Dropping database...");
        await context.Database.EnsureDeletedAsync();
        Console.WriteLine("✅ Database dropped.");
        break;

    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        Console.Error.WriteLine("Available commands: migrate | seed | setup | drop");
        Environment.Exit(1);
        break;
}
