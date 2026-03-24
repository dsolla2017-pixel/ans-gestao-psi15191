// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Minimal API Host] ASP.NET Core 8 — Top-level Statements:
//   Utiliza o modelo simplificado de Program.cs introduzido no
//   .NET 6, eliminando a classe Startup e reduzindo boilerplate.
//
// [Composition Root] Mark Seemann — Dependency Injection in .NET:
//   Todos os serviços são registrados no container de DI neste
//   único ponto de composição, garantindo inversão de controle.
//
// [Environment-Aware Configuration] Twelve-Factor App (Factor III):
//   A connection string é lida de configuração externa, com
//   fallback para SQLite em ambiente de desenvolvimento.
//
// [OpenAPI/Swagger] Documentação interativa da API gerada
//   automaticamente, facilitando integração com o front-end.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Caixa.Ans.Application.Services;
using Caixa.Ans.Domain.Interfaces;
using Caixa.Ans.Infrastructure.Data;
using Caixa.Ans.Infrastructure.Repositories;
using Caixa.Ans.Api.Middleware;

// ── Inicialização do Host ──────────────────────────────────────
// WebApplicationBuilder configura o host, serviços e pipeline.
var builder = WebApplication.CreateBuilder(args);

// ================================================================
// REGISTRO DE SERVIÇOS (Composition Root — Dependency Injection)
// ================================================================

// ── Banco de Dados ─────────────────────────────────────────────
// Estratégia dual: SQL Server em produção, SQLite em desenvolvimento.
// A connection string é lida de appsettings.json ou variável de ambiente,
// seguindo o princípio de configuração externalizada (Twelve-Factor App).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback para SQLite — permite execução local sem SQL Server instalado.
    // O arquivo ans_gestao.db é criado automaticamente pelo EF Core.
    builder.Services.AddDbContext<AnsDbContext>(options =>
        options.UseSqlite("Data Source=ans_gestao.db"));
}
else
{
    // Produção: SQL Server com connection string configurada.
    builder.Services.AddDbContext<AnsDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// ── Repositórios e Serviços ────────────────────────────────────
// Scoped lifetime: uma instância por requisição HTTP.
// IAcordoRepository → AcordoRepository (Dependency Inversion Principle).
builder.Services.AddScoped<IAcordoRepository, AcordoRepository>();
builder.Services.AddScoped<AcordoService>();

// ── Controllers e Documentação OpenAPI ─────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API de Gestão de ANS — GEGOD/CAIXA",
        Version = "v1",
        Description = "API RESTful para gerenciamento de Acordos de Nível de Serviço " +
                      "entre a CAIXA e empresas do Conglomerado. " +
                      "Documentação completa: https://gegodtransformacaodosdados.org"
    });
});

// ── CORS (Cross-Origin Resource Sharing) ───────────────────────
// Permite que o front-end Angular (porta 4200) acesse a API.
// Em produção, as origens devem ser restritas ao domínio real.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Build do Application ───────────────────────────────────────
var app = builder.Build();

// ================================================================
// PIPELINE DE REQUISIÇÕES HTTP (ordem importa)
// ================================================================

// 1. Middleware global de tratamento de erros (primeiro da fila).
//    Captura exceções não tratadas e retorna RFC 7807 Problem Details.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Swagger — documentação interativa da API.
//    Disponível apenas em desenvolvimento e homologação por segurança.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. Redirecionamento HTTPS — força conexões seguras.
app.UseHttpsRedirection();

// 4. CORS — aplica a política de origens permitidas.
app.UseCors("AllowAngular");

// 5. Autorização — valida tokens JWT nas rotas protegidas.
app.UseAuthorization();

// 6. Mapeamento de controllers — registra as rotas dos endpoints.
app.MapControllers();

// ── Inicialização do Banco de Dados ────────────────────────────
// Cria o schema automaticamente em desenvolvimento (EnsureCreated).
// Em produção, utilizar migrations do EF Core para controle de versão.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnsDbContext>();
    db.Database.EnsureCreated();
}

// ── Execução do Host ───────────────────────────────────────────
app.Run();
