using Microsoft.OpenApi.Models;
using MonitorEconomic.Infra.Ioc;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// 1️⃣ Adiciona controllers
// -------------------------------
builder.Services.AddControllers();

// -------------------------------
// 2️⃣ Configura Swagger
// -------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MonitorEconomic API",
        Version = "v1",
        Description = "API para consultar e armazenar IPC"
    });
});

// -------------------------------
// 3️⃣ Adiciona Injeção de Dependências (IoC)
// -------------------------------
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// -------------------------------
// 4️⃣ Middlewares Swagger (somente dev)
// -------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MonitorEconomic API V1");
        c.RoutePrefix = string.Empty; // Swagger no root
    });
}

// -------------------------------
// 5️⃣ Middlewares comuns
// -------------------------------
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Mapeia seus controllers, incluindo IPCController

app.Run();