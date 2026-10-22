
using System.Net;
using Inventory.Api.Tests;
using Xunit;

namespace Inventory.Api.Tests;

public class ProductsApiTests
{
    [Fact]
    public async Task GetProducts_Returns200OK()
    {
        // Arrange: create the test application.
        await using var factory = new InventoryApiFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        // Act: send a request to the real API.
        var response = await client.GetAsync("/api/products");

        // Assert: verify the HTTP status.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
