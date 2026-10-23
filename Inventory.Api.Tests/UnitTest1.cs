
using System.Net;
using System.Net.Http.Json;
using Inventory.Api.Tests;
using Xunit;

namespace Inventory.Api.Tests;

public class ProductsApiTests
{
    [Fact]
    public async Task GetProducts_Returns200OK()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_Returns201Created()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var product = new
        {
            sku = "TEST-001",
            name = "Test Keyboard",
            unitPrice = 1500.00m,
            quantityOnHand = 10,
            lowStockThreshold = 2
        };

        var response = await client.PostAsJsonAsync(
            "/api/products",
            product);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }



    [Fact]
    public async Task CreateProduct_WithDuplicateSku_Returns409Conflict()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var product = new
        {
            sku = "DUP-001",
            name = "Keyboard",
            unitPrice = 1500.00m,
            quantityOnHand = 10,
            lowStockThreshold = 2
        };

        var firstResponse = await client.PostAsJsonAsync(
            "/api/products", product);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/products", product);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithNegativePrice_Returns400BadRequest()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var product = new
        {
            sku = "INVALID-001",
            name = "Keyboard",
            unitPrice = -100.00m,
            quantityOnHand = 10,
            lowStockThreshold = 2
        };

        var response = await client.PostAsJsonAsync(
            "/api/products", product);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task GetProductById_WhenProductDoesNotExist_Returns404()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductExists_Returns200OK()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var originalProduct = new
        {
            sku = "UPDATE-001",
            name = "Keyboard",
            unitPrice = 1500.00m,
            quantityOnHand = 10,
            lowStockThreshold = 2
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/products", originalProduct);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        var updatedProduct = new
        {
            sku = "UPDATE-001",
            name = "Mechanical Keyboard",
            unitPrice = 2500.00m,
            quantityOnHand = 15,
            lowStockThreshold = 3
        };

        var updateResponse = await client.PutAsJsonAsync(
            location, updatedProduct);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync(location);
        getResponse.EnsureSuccessStatusCode();

        var savedProduct = await getResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(savedProduct);
        Assert.Equal("Mechanical Keyboard", savedProduct.Name);
        Assert.Equal(2500.00m, savedProduct.UnitPrice);
        Assert.Equal(15, savedProduct.QuantityOnHand);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductExists_Returns204AndRemovesProduct()
    {
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var product = new
        {
            sku = "DELETE-001",
            name = "Mouse",
            unitPrice = 800.00m,
            quantityOnHand = 20,
            lowStockThreshold = 5
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/products", product);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        var deleteResponse = await client.DeleteAsync(location);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync(location);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private sealed class ProductResponse
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int QuantityOnHand { get; set; }
        public int LowStockThreshold { get; set; }
    }

[Fact]
public async Task UpdateProduct_WhenProductDoesNotExist_Returns404()
{
    await using var factory = new InventoryApiFactory();
    await factory.InitializeDatabaseAsync();

    using var client = factory.CreateClient();

    var product = new
    {
        sku = "MISSING-001",
        name = "Keyboard",
        unitPrice = 1500.00m,
        quantityOnHand = 10,
        lowStockThreshold = 2
    };

    var response = await client.PutAsJsonAsync(
        "/api/products/99999", product);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}

[Fact]
public async Task DeleteProduct_WhenProductDoesNotExist_Returns404()
{
    await using var factory = new InventoryApiFactory();
    await factory.InitializeDatabaseAsync();

    using var client = factory.CreateClient();

    var response = await client.DeleteAsync("/api/products/99999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}

[Fact]
public async Task UpdateProduct_WithDuplicateSku_Returns409Conflict()
{
    await using var factory = new InventoryApiFactory();
    await factory.InitializeDatabaseAsync();

    using var client = factory.CreateClient();

    var firstProduct = new
    {
        sku = "SKU-001",
        name = "Keyboard",
        unitPrice = 1500.00m,
        quantityOnHand = 10,
        lowStockThreshold = 2
    };

    var secondProduct = new
    {
        sku = "SKU-002",
        name = "Mouse",
        unitPrice = 800.00m,
        quantityOnHand = 20,
        lowStockThreshold = 5
    };

    var firstResponse = await client.PostAsJsonAsync(
        "/api/products", firstProduct);

    var secondResponse = await client.PostAsJsonAsync(
        "/api/products", secondProduct);

    Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
    Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);

    var secondLocation = secondResponse.Headers.Location;
    Assert.NotNull(secondLocation);

    var updatedSecondProduct = new
    {
        sku = "SKU-001",
        name = "Updated Mouse",
        unitPrice = 900.00m,
        quantityOnHand = 25,
        lowStockThreshold = 5
    };

    var updateResponse = await client.PutAsJsonAsync(
        secondLocation, updatedSecondProduct);

    Assert.Equal(HttpStatusCode.Conflict, updateResponse.StatusCode);
}



}
