using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MonitorEconomic.Infra.Ioc;
//using MonitorEconomic.Infra.Data.Models;
using MonitorEconomic.Infra.Data.Entities;

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
builder.Services.AddDbContext<MonitorEconomicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// -------------------------------
// 4️⃣ Injeção de Dependência
// -------------------------------
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// -------------------------------
// 5️⃣ Executa migrations automaticamente
// -------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonitorEconomicDbContext>();
    db.Database.Migrate();
}

// -------------------------------
// 6️⃣ Swagger
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