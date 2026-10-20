namespace Inventory.Api.Entities;

public class Product
{
    public int id { get; set; }
    public string Sku { get; set; } = string.Empty;
    
    public string Name { get; set;} = string.Empty;

    public  decimal UnitPrice { get; set; }

    public int QuantityOnHand { get; set; }

    public int LowStockThreshold { get;  set; }

}