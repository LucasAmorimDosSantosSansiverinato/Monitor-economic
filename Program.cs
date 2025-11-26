using Monitor_economic._1_Monitor_econimic.Domain.Interfaces.IRepository;
using Monitor_economic._3_Monitor_economic.Infrastructure.Repository;
using Monitor_economic.Application.UseCases;

using Monitor_economic.Monitor_economic.Application.Interfaces.Service;
using Monitor_economic.Monitor_economic.Infrastructure.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================
// CONFIGURAÇÃO SUPABASE
// =======================
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
await supabaseClient.InitializeAsync();

builder.Services.AddSingleton(supabaseClient);

// =======================
// INJEÇÃO DE DEPENDÊNCIA
// =======================

// Repositório (Infrastructure)
builder.Services.AddScoped<IIPCRepository, IPCRepository>();

// Service da API Externa (Infrastructure)
builder.Services.AddHttpClient<IIPCService, IPCService>();

// UseCase (Application)
builder.Services.AddScoped<ObterIPCUseCase>();

// Controllers
builder.Services.AddControllers();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
