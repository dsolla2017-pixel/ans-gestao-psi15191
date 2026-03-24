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
//   A connection string e o provider de banco são lidos de
//   configuração externa, com fallback para SQLite em DEV.
//
// [JWT Bearer Authentication] RFC 7519 / OWASP:
//   Autenticação stateless via tokens JWT com validação de
//   issuer, audience e lifetime. Claims de role para RBAC.
//
// [RBAC] Role-Based Access Control:
//   Políticas de autorização baseadas em roles (Empregado,
//   Gerente) para controle granular de acesso aos endpoints.
//
// [OpenAPI/Swagger] Documentação interativa da API gerada
//   automaticamente, facilitando integração com o front-end.
// ============================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
// Estratégia dual controlada por appsettings:
//   - Development: SQLite (zero configuração, ideal para avaliação local)
//   - Production: SQL Server (via appsettings.Production.json)
//
// O provider é selecionado pela chave "DatabaseProvider" no appsettings.
// Se ausente, assume SQLite como fallback seguro (Twelve-Factor App).
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
    && !string.IsNullOrEmpty(connectionString))
{
    // Produção: SQL Server com connection string de appsettings.Production.json.
    // Schema alinhado com 02-database/01_create_database.sql (tabelas TB_ANS_*).
    builder.Services.AddDbContext<AnsDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            // Resiliência: retry automático em falhas transitórias de rede.
            // Referência: Microsoft — Connection Resiliency in EF Core.
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));
}
else
{
    // Desenvolvimento: SQLite — permite execução local sem SQL Server.
    // O arquivo ans_gestao.db é criado automaticamente pelo EF Core.
    builder.Services.AddDbContext<AnsDbContext>(options =>
        options.UseSqlite(connectionString ?? "Data Source=ans_gestao.db"));
}

// ── Repositórios e Serviços ────────────────────────────────────
// Scoped lifetime: uma instância por requisição HTTP.
// IAcordoRepository → AcordoRepository (Dependency Inversion Principle — SOLID "D").
builder.Services.AddScoped<IAcordoRepository, AcordoRepository>();
builder.Services.AddScoped<AcordoService>();

// ── Autenticação JWT Bearer (RFC 7519) ─────────────────────────
// Configuração de autenticação stateless via tokens JWT.
// Em desenvolvimento, utiliza chave simétrica de appsettings.json.
// Em produção, a chave DEVE ser armazenada em variável de ambiente
// ou Azure Key Vault (OWASP — Secrets Management).
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChaveDesenvolvimentoLocal_MinimoDe32Caracteres!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Caixa.Ans.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Caixa.Ans.Frontend";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Parâmetros de validação do token JWT.
    // ValidateIssuer/Audience: garante que o token foi emitido por esta API.
    // ValidateLifetime: rejeita tokens expirados.
    // IssuerSigningKey: chave simétrica para verificação da assinatura HMAC-SHA256.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        // Tolerância de clock para sincronização entre servidores.
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// ── Autorização RBAC (Role-Based Access Control) ───────────────
// Políticas de autorização baseadas em roles do token JWT.
// Roles disponíveis: "Empregado" (CRUD básico), "Gerente" (autorizar inativação).
builder.Services.AddAuthorization(options =>
{
    // Política padrão: qualquer usuário autenticado.
    options.AddPolicy("EmpregadoGEGOD", policy =>
        policy.RequireRole("Empregado", "Gerente"));

    // Política restrita: apenas gerentes podem autorizar inativação (dupla-custódia).
    options.AddPolicy("GerenteGEGOD", policy =>
        policy.RequireRole("Gerente"));
});

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
                      "Documentação completa: https://gegodtransformacaodosdados.org",
        Contact = new OpenApiContact
        {
            Name = "Diogo Grawingholt",
            Url = new Uri("https://www.diogograwingholt.com.br")
        }
    });

    // Configuração do Swagger para aceitar tokens JWT no header Authorization.
    // Permite testar endpoints protegidos diretamente pela interface Swagger UI.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── CORS (Cross-Origin Resource Sharing) ───────────────────────
// Permite que o front-end Angular (porta 4200) acesse a API.
// Em produção, as origens devem ser restritas ao domínio real.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://gegodtransformacaodosdados.org")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Build do Application ───────────────────────────────────────
var app = builder.Build();

// ================================================================
// PIPELINE DE REQUISIÇÕES HTTP (ordem importa — RFC 7230)
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

// 3. Redirecionamento HTTPS — força conexões seguras (HSTS).
app.UseHttpsRedirection();

// 4. CORS — aplica a política de origens permitidas.
app.UseCors("AllowAngular");

// 5. Autenticação — valida tokens JWT no header Authorization.
//    DEVE vir antes de UseAuthorization (ordem obrigatória ASP.NET Core).
app.UseAuthentication();

// 6. Autorização — verifica roles e políticas nos endpoints protegidos.
app.UseAuthorization();

// 7. Mapeamento de controllers — registra as rotas dos endpoints.
app.MapControllers();

// ── Inicialização do Banco de Dados ────────────────────────────
// Cria o schema automaticamente em desenvolvimento (EnsureCreated).
// Em produção, utilizar o script 02-database/01_create_database.sql
// ou migrations do EF Core para controle de versão do schema.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnsDbContext>();
    db.Database.EnsureCreated();
}

// ── Execução do Host ───────────────────────────────────────────
// Inicia o servidor Kestrel (DEV) ou IIS (PROD via web.config).
app.Run();
