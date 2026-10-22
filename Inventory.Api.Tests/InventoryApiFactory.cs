

using Inventory.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inventory.Api.Tests;

public class InventoryApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Remove the production DbContext registration.
            services.RemoveAll<InventoryDbContext>();
            services.RemoveAll<DbContextOptions<InventoryDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<InventoryDbContext>>();

            // Remove the original EF Core scoped lease and pool services.
            var efServices = services
                .Where(service =>
                    service.ServiceType.Namespace ==
                        "Microsoft.EntityFrameworkCore.Internal" &&
                    service.ServiceType.GenericTypeArguments
                        .Any(type => type == typeof(InventoryDbContext)))
                .Select(service => service.ServiceType)
                .Distinct()
                .ToList();

            foreach (var serviceType in efServices)
            {
                services.RemoveAll(serviceType);
            }

            // Register a normal, non-pooled context for SQLite tests.
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<InventoryDbContext>();

        await db.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
