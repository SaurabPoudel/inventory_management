using Microsoft.EntityFrameworkCore;
using Inventory.Api.Entities;

namespace Inventory.Api.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
      
    }
    public DbSet<Product> Products { get; set;}
}