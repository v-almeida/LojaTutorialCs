using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using loja.data;
using Microsoft.EntityFrameworkCore;
using loja.services;
using loja.models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<LojaDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 26))));

// Adicionando serviços
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<UsuarioService>(); // Adicionando o serviço de usuário

var app = builder.Build();

// Configurações de ambiente
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Endpoints para produtos
app.MapGet("/produtos", async (ProductService productService) =>
{
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
});

app.MapGet("/produtos/{id}", async (int id, ProductService productService) =>
{
    var produto = await productService.GetProductByIdAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(produto);
});

app.MapPost("/produtos", async (Produto produto, ProductService productService) =>
{
    await productService.AddProductAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapPut("/produtos/{id}", async (int id, Produto produto, ProductService productService) =>
{
    if (id != produto.Id)
    {
        return Results.BadRequest("Product ID mismatch.");
    }

    await productService.UpdateProductAsync(produto);
    return Results.Ok();
});

app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
{
    await productService.DeleteProductAsync(id);
    return Results.Ok();
});

// Endpoints para clientes
app.MapGet("/clientes", async (ClientService clientService) =>
{
    var clientes = await clientService.GetAllClientsAsync();
    return Results.Ok(clientes);
});

app.MapGet("/clientes/{id}", async (int id, ClientService clientService) =>
{
    var cliente = await clientService.GetClientByIdAsync(id);
    if (cliente == null)
    {
        return Results.NotFound($"Client with ID {id} not found.");
    }
    return Results.Ok(cliente);
});

app.MapPost("/clientes", async (Cliente cliente, ClientService clientService) =>
{
    await clientService.AddClientAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
});

app.MapPut("/clientes/{id}", async (int id, Cliente cliente, ClientService clientService) =>
{
    if (id != cliente.Id)
    {
        return Results.BadRequest("Client ID mismatch.");
    }

    await clientService.UpdateClientAsync(cliente);
    return Results.Ok();
});

app.MapDelete("/clientes/{id}", async (int id, ClientService clientService) =>
{
    await clientService.DeleteClientAsync(id);
    return Results.Ok();
});

// Endpoints para fornecedores
app.MapGet("/fornecedores", async (FornecedorService fornecedorService) =>
{
    var fornecedores = await fornecedorService.GetAllFornecedoresAsync();
    return Results.Ok(fornecedores);
});

app.MapGet("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
{
    var fornecedor = await fornecedorService.GetFornecedorByIdAsync(id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Fornecedor with ID {id} not found.");
    }
    return Results.Ok(fornecedor);
});

app.MapPost("/fornecedores", async (Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    await fornecedorService.AddFornecedorAsync(fornecedor);
    return Results.Created($"/fornecedores/{fornecedor.Id}", fornecedor);
});

app.MapPut("/fornecedores/{id}", async (int id, Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    if (id != fornecedor.Id)
    {
        return Results.BadRequest("Fornecedor ID mismatch.");
    }

    await fornecedorService.UpdateFornecedorAsync(fornecedor);
    return Results.Ok();
});

app.MapDelete("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
{
    await fornecedorService.DeleteFornecedorAsync(id);
    return Results.Ok();
});

// Endpoints para usuários
app.MapGet("/usuarios", async (UsuarioService usuarioService) =>
{
    var usuarios = await usuarioService.GetAllUsuariosAsync();
    return Results.Ok(usuarios);
});

app.MapGet("/usuarios/{id}", async (int id, UsuarioService usuarioService) =>
{
    var usuario = await usuarioService.GetUsuarioByIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound($"Usuário with ID {id} not found.");
    }
    return Results.Ok(usuario);
});

app.MapPost("/usuarios", async (Usuario usuario, UsuarioService usuarioService) =>
{
    await usuarioService.AddUsuarioAsync(usuario);
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
});

app.MapPut("/usuarios/{id}", async (int id, Usuario usuario, UsuarioService usuarioService) =>
{
    if (id != usuario.Id)
    {
        return Results.BadRequest("Usuário ID mismatch.");
    }

    await usuarioService.UpdateUsuarioAsync(usuario);
    return Results.Ok();
});

app.MapDelete("/usuarios/{id}", async (int id, UsuarioService usuarioService) =>
{
    await usuarioService.DeleteUsuarioAsync(id);
    return Results.Ok();
});

// Endpoint para testar a conexão com o banco de dados
app.MapGet("/test-connection", async (LojaDbContext dbContext) =>
{
    try
    {
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.CloseConnectionAsync();
        return Results.Ok("Connection successful");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Connection failed: {ex.Message}");
    }
});

app.Run();













