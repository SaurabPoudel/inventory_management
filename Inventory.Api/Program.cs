using Microsoft.EntityFrameworkCore;
using Inventory.Api.Data;
using Inventory.Api.Entities;

var  builder  = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextPool<InventoryDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("InventoryDb")));


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }