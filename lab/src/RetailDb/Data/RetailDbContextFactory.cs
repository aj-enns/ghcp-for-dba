using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RetailDb.Data;

// Used by the EF Core CLI tools (dotnet ef migrations add / database update)
// when no running host is available to provide the DbContext.
public class RetailDbContextFactory : IDesignTimeDbContextFactory<RetailDbContext>
{
    public RetailDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("RETAILDB_CONNECTION_STRING")
            ?? "Server=tcp:localhost,1433;Initial Catalog=RetailDb;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<RetailDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new RetailDbContext(options);
    }
}
