# Inventory Management API

A small REST API for managing product inventory. It lets you create products, look them up, update their details, and remove them. Each product has a unique SKU, a price, an on-hand quantity, and a low-stock threshold.

The API is built with ASP.NET Core and stores data in PostgreSQL through Entity Framework Core.

## What it can do

- List all products or retrieve one by its ID.
- Create, update, and delete products.
- Validate required fields and reject invalid prices or negative stock values.
- Prevent duplicate SKUs. Creating or updating a product with an SKU already used by another product returns `409 Conflict`.
- Return `404 Not Found` when a requested product does not exist.

The low-stock threshold is stored with each product; the API does not currently provide stock alerts or inventory-adjustment endpoints.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL
- The `dotnet-ef` command-line tool for applying database migrations

Install the EF Core command-line tool if you do not already have it:

```sh
dotnet tool install --global dotnet-ef
```

## Run locally

1. Create a PostgreSQL database, for example `inventory_management`.
2. Set the connection string using the configuration key `ConnectionStrings:InventoryDb`. The environment-variable form shown here keeps credentials out of the repository:

   ```sh
   export ConnectionStrings__InventoryDb='Host=localhost;Port=5432;Database=inventory_management;Username=postgres;Password=your_password'
   ```

   Replace the username and password with values for your PostgreSQL instance.

3. Apply the migrations from the repository root:

   ```sh
   dotnet ef database update --project Inventory.Api --startup-project Inventory.Api
   ```

4. Start the API with the HTTP development profile:

   ```sh
   dotnet run --project Inventory.Api --launch-profile http
   ```

The API will be available at `http://localhost:5192`. In the Development environment, the OpenAPI document is available at `http://localhost:5192/openapi/v1.json`.

## API

All routes are under `/api/products`.

| Method | Route | Description | Success response |
| --- | --- | --- | --- |
| `GET` | `/api/products` | List products | `200 OK` |
| `GET` | `/api/products/{id}` | Get a product by ID | `200 OK` |
| `POST` | `/api/products` | Create a product | `201 Created` |
| `PUT` | `/api/products/{id}` | Replace a product's details | `200 OK` |
| `DELETE` | `/api/products/{id}` | Delete a product | `204 No Content` |

Create and update requests use this JSON shape:

```json
{
  "sku": "KB-002",
  "name": "Virtual Keyboard",
  "unitPrice": 45.00,
  "quantityOnHand": 20,
  "lowStockThreshold": 5
}
```

`sku` is required and limited to 50 characters. `name` is required and limited to 150 characters. `unitPrice` must be between `0.01` and `999999999`; `quantityOnHand` and `lowStockThreshold` must be zero or greater.

Example requests, with the API running locally:

```sh
curl http://localhost:5192/api/products

curl -X POST http://localhost:5192/api/products \
  -H 'Content-Type: application/json' \
  -d '{"sku":"KB-002","name":"Virtual Keyboard","unitPrice":45.00,"quantityOnHand":20,"lowStockThreshold":5}'

curl http://localhost:5192/api/products/1

curl -X PUT http://localhost:5192/api/products/1 \
  -H 'Content-Type: application/json' \
  -d '{"sku":"KB-002","name":"Virtual Keyboard","unitPrice":45.00,"quantityOnHand":18,"lowStockThreshold":5}'

curl -X DELETE http://localhost:5192/api/products/1
```

## Run the tests

From the repository root:

```sh
dotnet test Inventory.Api.Tests/Inventory.Api.Tests.csproj
```

The integration tests use an isolated SQLite database, so PostgreSQL does not need to be running to execute the test suite.
