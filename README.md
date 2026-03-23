# Sistema de Gestao de Acordos de Nivel de Servico (ANS)

**PSI 15191** | ASS Executivo Master - Desenvolvedor | CAIXA Economica Federal

**Autor:** Diogo Grawingholt | **Gerencia:** GEGOD - Gerencia Nacional de Governanca de Dados

---

## Visao Geral

Solucao web para registro e gerenciamento dos Acordos de Nivel de Servico (ANS) de compartilhamento de dados entre a CAIXA e as empresas do conglomerado. O sistema permite incluir, listar, consultar, editar e excluir ANS, com fluxo de assinatura digital (dupla-custodia), controle de vigencia e conformidade com LGPD e MN OR016.

## Stack Tecnologica

| Camada | Tecnologia | Versao |
|--------|-----------|--------|
| Back-end | C# / ASP.NET Core | 8.0 |
| Front-end | Angular + DSC CAIXA | 17 |
| Banco de Dados | SQL Server / SQLite | 2022 / 3 |
| Servidor Web | IIS (Kestrel em dev) | 10 |
| ORM | Entity Framework Core | 8.0 |

## Estrutura do Projeto

```
ans-gestao/
  backend/
    src/
      Caixa.Ans.Api/           # Controllers, Middleware, Program.cs
      Caixa.Ans.Application/   # Services, DTOs
      Caixa.Ans.Domain/        # Entidades, Enums, Interfaces
      Caixa.Ans.Infrastructure/# DbContext, Repositories
    tests/
      Caixa.Ans.UnitTests/     # Testes unitarios (xUnit)
  frontend/
    src/app/
      core/                    # Services, Guards, Interceptors
      features/acordos/        # Componentes de tela
      shared/models/           # Interfaces TypeScript
  database/
    01_create_database.sql     # Script de criacao do banco
    02_seed_data.sql           # Massa de dados para teste
  scripts/
    run-local.bat              # Execucao local (Windows)
    run-local.ps1              # Execucao local (PowerShell)
    deploy-iis.bat             # Deploy em IIS
    web.config                 # Configuracao do IIS
  docs/
    PROPOSTA_DESAFIO.md        # Proposta tecnica completa
    JORNADA_DO_AVALIADOR.md    # Guia para o avaliador
    CHECKLIST_DESAFIO_VS_SUPERACAO.md
    VISAO_FUTURO.md            # Visao de futuro e inovacao
```

## Execucao Local

### Pre-requisitos

- .NET 8 SDK
- Node.js 18+
- Angular CLI (`npm install -g @angular/cli`)

### Opcao 1: Script automatizado (Windows)

```batch
cd scripts
run-local.bat
```

### Opcao 2: PowerShell

```powershell
cd scripts
.\run-local.ps1
```

### Opcao 3: Manual

```bash
# Back-end
cd backend/src/Caixa.Ans.Api
dotnet restore
dotnet run --urls=http://localhost:5000

# Front-end (em outro terminal)
cd frontend
npm install
npx ng serve --port 4200
```

## Banco de Dados

### Configuracao SQL Server (Producao)

1. Execute `database/01_create_database.sql` no SSMS
2. Execute `database/02_seed_data.sql` para massa de teste
3. Configure a connection string em `appsettings.json`

### SQLite (Desenvolvimento)

O banco SQLite e criado automaticamente ao iniciar a aplicacao em modo desenvolvimento. Nenhuma configuracao adicional e necessaria.

## Deploy em IIS

```batch
cd scripts
deploy-iis.bat C:\inetpub\wwwroot\ans-gestao
```

## Criterios de Avaliacao Atendidos

- **Uso das tecnologias solicitadas:** C#/.NET 8, Angular 17, SQL Server, Node.js, DSC CAIXA
- **Manutenibilidade e clareza de codigo:** Clean Architecture, SOLID, DDD, comentarios didaticos
- **Alcance do objetivo proposto:** CRUD completo, assinatura, inativacao, exclusao
- **Boas praticas:** Testes unitarios, Git Flow, auditoria de autoria, documentacao

## Licenca

Projeto desenvolvido exclusivamente para o PSI 15191 da CAIXA Economica Federal.
