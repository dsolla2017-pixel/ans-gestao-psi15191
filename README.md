# Sistema de Gestão de ANS — PSI 15191

**Autor:** Diogo Grawingholt | Gerência Nacional de Governança de Dados (GEGOD) — CAIXA

Solução web para registro e gestão de Acordos de Nível de Serviço (ANS) de compartilhamento de dados entre a CAIXA e as empresas do conglomerado.

---

## Fluxo de Boas Práticas — Estrutura do Projeto

O repositório segue uma sequência numerada que reflete o fluxo natural de desenvolvimento, da configuração inicial até a documentação final. Cada pasta corresponde a uma etapa do ciclo de vida do projeto.

| Etapa | Pasta | Descrição |
|-------|-------|-----------|
| 1 | `01-setup/` | Scripts de configuração local, execução e auditoria de autoria |
| 2 | `02-database/` | DDL de criação do banco de dados e massa de dados para testes |
| 3 | `03-backend/` | Código-fonte C#/.NET 8 — Clean Architecture em 5 camadas |
| 4 | `04-frontend/` | Código-fonte Angular 17 — componentes, serviços, guards e interceptors |
| 5 | `05-tests/` | Testes unitários com xUnit cobrindo regras de negócio do domínio |
| 6 | `06-deploy/` | Scripts de deploy para IIS e configuração do servidor web |
| 7 | `07-docs/` | Documentação estratégica: proposta, jornada do avaliador, checklist e visão de futuro |

---

## 1. Setup (`01-setup/`)

Contém os scripts necessários para configurar e executar o projeto localmente.

| Arquivo | Finalidade |
|---------|-----------|
| `run-local.bat` | Execução local via CMD (Windows) |
| `run-local.ps1` | Execução local via PowerShell |
| `audit_authorship.py` | Script Python que valida headers de autoria em todos os arquivos |

**Pré-requisitos:** .NET 8 SDK, Node.js 18+, Angular CLI.

```bash
# Via CMD
01-setup\run-local.bat

# Via PowerShell
01-setup\run-local.ps1
```

---

## 2. Database (`02-database/`)

Scripts SQL para criação do banco e carga de dados de teste.

| Arquivo | Finalidade |
|---------|-----------|
| `01_create_database.sql` | DDL completa: tabelas, índices, constraints e triggers |
| `02_seed_data.sql` | Massa de dados com 5 acordos para validação funcional |

Execute os scripts na ordem numérica em uma instância SQL Server. Para desenvolvimento, o SQLite é criado automaticamente ao iniciar a aplicação.

---

## 3. Backend (`03-backend/`)

API RESTful desenvolvida em C#/.NET 8 com Clean Architecture.

```
03-backend/src/
├── Caixa.Ans.Domain/          ← Entidades, Enums e Interfaces (camada mais interna)
├── Caixa.Ans.Application/     ← DTOs e Serviços de aplicação
├── Caixa.Ans.Infrastructure/  ← DbContext e Repositórios (Entity Framework Core)
└── Caixa.Ans.Api/             ← Controllers, Middleware e Program.cs
```

**Princípios aplicados:** SOLID, DDD, Repository Pattern, inversão de dependência.

---

## 4. Frontend (`04-frontend/`)

Interface web desenvolvida em Angular 17 com Design System CAIXA (DSC).

```
04-frontend/src/app/
├── core/
│   ├── guards/         ← AuthGuard para proteção de rotas
│   ├── interceptors/   ← JWT Interceptor para autenticação
│   └── services/       ← AcordoService (comunicação HTTP com a API)
├── features/
│   └── acordos/        ← Componentes de listagem e formulário
└── shared/
    └── models/         ← Interfaces TypeScript do domínio
```

---

## 5. Tests (`05-tests/`)

Testes unitários com xUnit que validam as regras de negócio do domínio.

| Arquivo | Cobertura |
|---------|-----------|
| `AcordoEntityTests.cs` | Regras de ativação, inativação, exclusão e transições de estado |

---

## 6. Deploy (`06-deploy/`)

Scripts e configurações para publicação em servidor IIS.

| Arquivo | Finalidade |
|---------|-----------|
| `deploy-iis.bat` | Script automatizado de deploy para IIS |
| `web.config` | Configuração do servidor web (rewrite rules, MIME types) |

```batch
06-deploy\deploy-iis.bat C:\inetpub\wwwroot\ans-gestao
```

---

## 7. Documentação (`07-docs/`)

Documentação estratégica que acompanha a entrega técnica.

| Documento | Conteúdo |
|-----------|---------|
| `PROPOSTA_DESAFIO.md` | Proposta técnica completa com arquitetura, roadmap e diferenciais |
| `JORNADA_DO_AVALIADOR.md` | Guia passo-a-passo para o avaliador percorrer a entrega |
| `CHECKLIST_DESAFIO_VS_SUPERACAO.md` | Comparativo requisito vs. entrega vs. superação |
| `VISAO_FUTURO.md` | Roadmap de evolução com IA, Power BI e Microsoft Fabric |

---

## Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Back-end | C# / .NET 8 / ASP.NET Core |
| Front-end | Angular 17 / TypeScript / Design System CAIXA |
| Banco de Dados | SQL Server / SQLite (dev) |
| Servidor Web | IIS (produção) / Kestrel (desenvolvimento) |
| Testes | xUnit / Moq |
| Versionamento | Git / Git Flow |

---

## Critérios de Avaliação Atendidos

| Critério | Como foi atendido |
|----------|------------------|
| Uso das tecnologias solicitadas | C#/.NET 8, Angular 17, SQL Server, Node.js, DSC CAIXA |
| Manutenibilidade e clareza de código | Clean Architecture, SOLID, DDD, comentários em português |
| Alcance do objetivo proposto | Todas as funcionalidades do desafio implementadas |
| Técnicas adequadas e melhores práticas | Repository Pattern, JWT, Guards, Interceptors, testes unitários |

---

**Diogo Grawingholt** | GEGOD — CAIXA Econômica Federal | 2025
