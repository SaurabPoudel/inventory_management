using Inventory.Api.Data;
using Inventory.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Inventory.Api.DTOs;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public ProductsController(InventoryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(
        CreateProductRequest request
    )
    {
        var sku = request.Sku.Trim();

        var skuExists = await _context.Products
            .AnyAsync(p => p.Sku == sku);

        if (skuExists)
        {
            return Conflict(new
            {
                message = $"A product with SKU '{sku}' already exists."
            });
        }
        var product = new Product
        {
            Sku = sku,
            Name = request.Name.Trim(),
            UnitPrice = request.UnitPrice,
            QuantityOnHand = request.QuantityOnHand,
            LowStockThreshold = request.LowStockThreshold
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
    nameof(GetProductById),
    new { id = product.id },
    product
);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPut("{id:int}")]
public async Task<ActionResult<Product>> UpdateProduct(
    int id,
    CreateProductRequest request)
{
    var product = await _context.Products.FindAsync(id);

    if (product == null)
    {
        return NotFound();
    }

    var sku = request.Sku.Trim();

    var skuExists = await _context.Products
        .AnyAsync(p => p.Sku == sku && p.id != id);

    if (skuExists)
    {
        return Conflict(new
        {
            message = $"A product with SKU '{sku}' already exists."
        });
    }

    product.Sku = sku;
    product.Name = request.Name.Trim();
    product.UnitPrice = request.UnitPrice;
    product.QuantityOnHand = request.QuantityOnHand;
    product.LowStockThreshold = request.LowStockThreshold;

    await _context.SaveChangesAsync();

    return Ok(product);
}

[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteProduct(int id)
{
    var product = await _context.Products.FindAsync(id);

    if (product == null)
    {
        return NotFound();
    }

    _context.Products.Remove(product);

    await _context.SaveChangesAsync();

    return NoContent();
}
}

