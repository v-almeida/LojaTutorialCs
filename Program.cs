using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the connection to the DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}"); // For debugging

builder.Services.AddDbContext<LojaDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))));

var app = builder.Build();

// Run migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LojaDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/createproduto", async (LojaDbContext dbContext, Produto newProduto) =>
{
    dbContext.Produtos.Add(newProduto);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createproduto{newProduto.Id}", newProduto);
});

app.MapGet("/produtos", async (LojaDbContext dbContext)=>
{
    var produtos = await dbContext.Produtos.ToListAsync();
    return Results.Ok(produtos);
});

app.MapGet("/produtos/{id}", async (int id, LojaDbContext dbContext) =>
{
    var produto = await dbContext.Produtos.FindAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Produto with ID {id} not found.");
    }

    return Results.Ok(produto);
});

app.MapPut("/produtos/{id}", async (int id, LojaDbContext dbContext, Produto updateProduto) =>
{
    var existingproduto = await dbContext.Produtos.FindAsync(id);
    if (existingproduto == null)
    {
        return Results.NotFound($"Produto with ID {id} not found.");
    }

    existingproduto.Nome = updateProduto.Nome;
    existingproduto.Preco = updateProduto.Preco;
    existingproduto.Fornecedor = updateProduto.Fornecedor;

    await dbContext.SaveChangesAsync();

    return Results.Ok(existingproduto);
});



var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    
}



