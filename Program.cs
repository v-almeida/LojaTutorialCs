using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using loja.data;
using Microsoft.EntityFrameworkCore;
using loja.services;
using loja.models;
using System.IdentityModel.Tokens.Jwt;

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

// Configuração da autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abc123ABC456")) // Chave secreta para validar o token
        };
    });

var app = builder.Build();

// Configurações de ambiente
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Middleware para autenticação
app.UseAuthentication();

// Middleware para redirecionamento HTTPS
app.UseHttpsRedirection();

// Endpoints para produtos
app.MapGet("/produtos", async (ProductService productService) =>
{
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapGet("/produtos/{id}", async (int id, ProductService productService) =>
{
    var produto = await productService.GetProductByIdAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(produto);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPost("/produtos", async (Produto produto, ProductService productService) =>
{
    await productService.AddProductAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPut("/produtos/{id}", async (int id, Produto produto, ProductService productService) =>
{
    if (id != produto.Id)
    {
        return Results.BadRequest("Product ID mismatch.");
    }

    await productService.UpdateProductAsync(produto);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
{
    await productService.DeleteProductAsync(id);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

// Endpoints para clientes
app.MapGet("/clientes", async (ClientService clientService) =>
{
    var clientes = await clientService.GetAllClientsAsync();
    return Results.Ok(clientes);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapGet("/clientes/{id}", async (int id, ClientService clientService) =>
{
    var cliente = await clientService.GetClientByIdAsync(id);
    if (cliente == null)
    {
        return Results.NotFound($"Client with ID {id} not found.");
    }
    return Results.Ok(cliente);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPost("/clientes", async (Cliente cliente, ClientService clientService) =>
{
    await clientService.AddClientAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPut("/clientes/{id}", async (int id, Cliente cliente, ClientService clientService) =>
{
    if (id != cliente.Id)
    {
        return Results.BadRequest("Client ID mismatch.");
    }

    await clientService.UpdateClientAsync(cliente);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

app.MapDelete("/clientes/{id}", async (int id, ClientService clientService) =>
{
    await clientService.DeleteClientAsync(id);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

// Endpoints para fornecedores
app.MapGet("/fornecedores", async (FornecedorService fornecedorService) =>
{
    var fornecedores = await fornecedorService.GetAllFornecedoresAsync();
    return Results.Ok(fornecedores);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapGet("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
{
    var fornecedor = await fornecedorService.GetFornecedorByIdAsync(id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Fornecedor with ID {id} not found.");
    }
    return Results.Ok(fornecedor);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPost("/fornecedores", async (Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    await fornecedorService.AddFornecedorAsync(fornecedor);
    return Results.Created($"/fornecedores/{fornecedor.Id}", fornecedor);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPut("/fornecedores/{id}", async (int id, Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    if (id != fornecedor.Id)
    {
        return Results.BadRequest("Fornecedor ID mismatch.");
    }

    await fornecedorService.UpdateFornecedorAsync(fornecedor);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

app.MapDelete("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
{
    await fornecedorService.DeleteFornecedorAsync(id);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

// Endpoint para usuários
app.MapGet("/usuarios", async (UsuarioService usuarioService) =>
{
    var usuarios = await usuarioService.GetAllUsuariosAsync();
    return Results.Ok(usuarios);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapGet("/usuarios/{id}", async (int id, UsuarioService usuarioService) =>
{
    var usuario = await usuarioService.GetUsuarioByIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound($"Usuário with ID {id} not found.");
    }
    return Results.Ok(usuario);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPost("/usuarios", async (HttpContext context, UsuarioService usuarioService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var json = JsonDocument.Parse(body);
    var username = json.RootElement.GetProperty("username").GetString();
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    // Lógica para adicionar usuário
    var usuario = new Usuario { Nome = username, Email = email, Senha = senha };
    await usuarioService.AddUsuarioAsync(usuario);
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
}).RequireAuthorization(); // Requer autenticação JWT

app.MapPut("/usuarios/{id}", async (int id, Usuario usuario, UsuarioService usuarioService) =>
{
    if (id != usuario.Id)
    {
        return Results.BadRequest("Usuário ID mismatch.");
    }

    await usuarioService.UpdateUsuarioAsync(usuario);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

app.MapDelete("/usuarios/{id}", async (int id, UsuarioService usuarioService) =>
{
    await usuarioService.DeleteUsuarioAsync(id);
    return Results.Ok();
}).RequireAuthorization(); // Requer autenticação JWT

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

// Endpoint para login
app.MapPost("/login", async (HttpContext context) =>
{
    // Receber o request
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var json = JsonDocument.Parse(body);
    var username = json.RootElement.GetProperty("username").GetString();
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    // Lógica de autenticação (exemplo simples)
    var token = "";
    if (senha == "1029")
    {
        token = GenerateToken(email); // Método GenerateToken a ser implementado
    }

    // Retornar o token
    await context.Response.WriteAsync(token);
});

// Rota segura: verificar se o token está presente e validar
app.MapGet("/rotaSegura", async (HttpContext context) =>
{
    // Verificar se o token está presente
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token não fornecido");
        return;
    }

    // Obter o token
    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    // Validar o token
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("abc123ABC456"); // Chave secreta (deve ser a mesma usada para gerar o token)
    var validationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    SecurityToken validateToken;
    try
    {
        // Decodificar, verificar e validar o token
        tokenHandler.ValidateToken(token, validationParameters, out validateToken);
    }
    catch (Exception)
    {
        // Caso o token seja inválido
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token inválido");
        return;
    }

    // Se o token é válido: continuar com a lógica do endpoint
    await context.Response.WriteAsync("Autorizado");
});

// Método para gerar token JWT (exemplo)
string GenerateToken(string email)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("abc123ABC456");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new System.Security.Claims.ClaimsIdentity(new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email)
        }),
        Expires = DateTime.UtcNow.AddHours(1), // Tempo de expiração do token
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.Run();




















