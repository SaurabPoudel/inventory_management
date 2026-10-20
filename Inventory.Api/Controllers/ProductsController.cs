using Inventory.Api.Data;
using Inventory.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Inventory.Api.DTOs;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController  :  ControllerBase
{
    private readonly InventoryDbContext  _context;

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
    public  async Task<ActionResult<Product>> CreateProduct (
        CreateProductRequest request
    )
    {
        var product = new Product
        {
            Sku  = request.Sku.Trim(),
            Name  = request.Name.Trim(),
            UnitPrice  = request.UnitPrice,
            QuantityOnHand = request.QuantityOnHand,
            LowStockThreshold = request.LowStockThreshold
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProducts),
            new  { id = product.id},
            product
        );

        
    
    }
}