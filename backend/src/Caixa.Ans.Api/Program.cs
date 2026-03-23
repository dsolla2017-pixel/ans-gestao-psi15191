// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Program.cs: Arquivo principal que inicia a aplicacao.
// - Dependency Injection: Sistema que conecta as pecas automaticamente.
// - Swagger: Ferramenta que gera documentacao interativa da API.
// - CORS: Politica de seguranca que permite o front-end acessar a API.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Caixa.Ans.Application.Services;
using Caixa.Ans.Domain.Interfaces;
using Caixa.Ans.Infrastructure.Data;
using Caixa.Ans.Infrastructure.Repositories;
using Caixa.Ans.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// CONFIGURACAO DE SERVICOS (Dependency Injection)
// ============================================================

// Banco de dados: SQL Server em producao, SQLite em desenvolvimento
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback para SQLite (desenvolvimento local)
    builder.Services.AddDbContext<AnsDbContext>(options =>
        options.UseSqlite("Data Source=ans_gestao.db"));
}
else
{
    builder.Services.AddDbContext<AnsDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Repositorios e servicos
builder.Services.AddScoped<IAcordoRepository, AcordoRepository>();
builder.Services.AddScoped<AcordoService>();

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API de Gestao de ANS - CAIXA",
        Version = "v1",
        Description = "API RESTful para gerenciamento de Acordos de Nivel de Servico " +
                      "entre a CAIXA e empresas do conglomerado."
    });
});

// CORS (permite o front-end Angular acessar a API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============================================================
// PIPELINE DE REQUISICOES
// ============================================================

// Middleware global de tratamento de erros (primeiro da fila)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger (disponivel em desenvolvimento e homologacao)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

// Criar banco automaticamente em desenvolvimento
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnsDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
