
using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.DTOs;

public class CreateProductRequest
{
    [Required]
    [StringLength(50)]
    public  string Sku { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public  string Name  { get;  set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999")]
    public  decimal  UnitPrice  {get;  set;}

    [Range(0, int.MaxValue)]
    public int QuantityOnHand { get; set;}

    [Range(0, int.MaxValue)]
    public  int LowStockThreshold { get; set;}
}