using Microsoft.OpenApi.Models;
using MonitorEconomic.Infra.Ioc;
using MonitorEconomic.Infrastructure.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// 1️⃣ Controllers
// -------------------------------
builder.Services.AddControllers();

// -------------------------------
// 2️⃣ Swagger
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
// 3️⃣ DbContext
// -------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped(sp => new MonitorEconomicDbContext(connectionString ?? throw new InvalidOperationException("Connection string is null")));

// -------------------------------
// 4️⃣ Injeção de Dependência
// -------------------------------
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// -------------------------------
// 5️⃣ Swagger
// -------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MonitorEconomic API V1");
        c.RoutePrefix = string.Empty;
    });
}

// -------------------------------
// 7️⃣ Middlewares
// -------------------------------
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();