# Proposta Técnica: Sistema de Gestão de Acordos de Nível de Serviço (ANS)

> **Autor:** Diogo Grawingholt | **Projeto:** Transformação Digital na Governança de Dados — GEGOD/CAIXA
> **Site:** [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org) | **Portfólio:** [diogograwingholt.com.br](https://www.diogograwingholt.com.br)
> **PSI:** 15191 — ASS Executivo Master — Papel: Desenvolvedor

A presente proposta técnica descreve a arquitetura, modelagem, fluxos e tecnologias adotadas para a construção do Sistema de Gestão de Acordos de Nível de Serviço (ANS). O sistema atende integralmente ao desafio proposto no Processo Seletivo Interno (PSI) 15191 para a Gerência Nacional de Governança e Inteligência de Dados (GEGOD).

A solução assegura a integridade, segurança e usabilidade na gestão de compartilhamento de dados entre a CAIXA e as empresas do seu conglomerado. A arquitetura segue padrões corporativos modernos, garantindo alta disponibilidade, manutenibilidade e escalabilidade, alinhada às melhores práticas de engenharia de software e ao mapa estratégico da instituição.

## 1. Visão Geral da Solução

O Sistema de Gestão de ANS é uma aplicação web completa que digitaliza o processo de criação, aprovação e acompanhamento de acordos operacionais para compartilhamento de dados. A plataforma elimina controles manuais e planilhas, centralizando as informações em um repositório seguro e auditável.

A solução implementa fluxos de dupla-custódia para inativação de acordos, validações de segurança da informação (grau de sigilo e dados sensíveis) e assinaturas digitais dos responsáveis. A interface de usuário, construída com o Design System CAIXA (DSC), oferece uma experiência fluida, acessível e responsiva.

## 2. Objetivos do Sistema

Os principais objetivos da plataforma incluem:

- **Centralizar a gestão**: Criar um repositório único e confiável para todos os Acordos de Nível de Serviço do conglomerado.
- **Garantir conformidade**: Assegurar que os compartilhamentos respeitem as normativas internas, especialmente o MN OR016, identificando dados pessoais e sensíveis.
- **Automatizar fluxos**: Implementar assinaturas digitais e aprovações em dupla-custódia, reduzindo o tempo de tramitação.
- **Prover rastreabilidade**: Manter histórico completo de alterações, inativações e acessos, essencial para auditorias.
- **Oferecer usabilidade superior**: Entregar uma interface intuitiva, rápida e alinhada à identidade visual da CAIXA.

## 3. Arquitetura Proposta

A arquitetura adota o padrão **Clean Architecture** (Arquitetura Limpa), separando as responsabilidades em camadas independentes. Essa abordagem facilita a manutenção, permite a criação de testes automatizados eficazes e isola as regras de negócio de detalhes de infraestrutura.

| Camada | Responsabilidade | Tecnologias Empregadas |
| :--- | :--- | :--- |
| **Apresentação (Front-end)** | Interface do usuário, interações, validações visuais e consumo de APIs. | Angular, TypeScript, HTML5, SCSS, Design System CAIXA (DSC). |
| **API (Back-end)** | Exposição de endpoints RESTful, roteamento, autenticação e autorização. | C# .NET 8 (ASP.NET Core Web API), Swagger/OpenAPI. |
| **Aplicação (Application)** | Casos de uso, orquestração de regras de negócio, DTOs e mapeamentos. | C#, MediatR (CQRS), AutoMapper, FluentValidation. |
| **Domínio (Domain)** | Entidades, objetos de valor, interfaces de repositórios e regras de negócio puras. | C# (Classes POCO, sem dependências externas). |
| **Infraestrutura (Infrastructure)** | Acesso a dados, integrações externas, logs e serviços de mensageria. | Entity Framework Core, SQL Server / SQLite, Serilog. |

A aplicação será hospedada no **Servidor Web IIS**, com o banco de dados gerenciado pelo **SQL Server** em ambiente de produção (com suporte a SQLite para desenvolvimento e testes locais).

## 4. Justificativa das Tecnologias Adotadas

As escolhas tecnológicas atendem aos requisitos obrigatórios do edital e incorporam padrões de mercado para sistemas corporativos:

- **C# / .NET 8**: Plataforma robusta, madura e amplamente adotada na CAIXA. Oferece excelente performance, tipagem forte e um ecossistema rico para construção de APIs seguras.
- **Angular**: Framework front-end estruturado que facilita a criação de Single Page Applications (SPAs) complexas. Sua arquitetura baseada em componentes integra-se perfeitamente ao Design System CAIXA.
- **SQL Server**: Banco de dados relacional corporativo, capaz de lidar com grandes volumes de dados, transações complexas e requisitos rigorosos de segurança e backup.
- **Entity Framework Core**: ORM (Object-Relational Mapper) oficial da Microsoft que acelera o desenvolvimento da camada de dados e permite a migração segura de esquemas (Code-First).
- **Design System CAIXA (DSC)**: Garante a padronização visual, acessibilidade e conformidade com as diretrizes de marca da instituição.

## 5. Módulos e Funcionalidades

O sistema é composto por módulos interligados que cobrem todo o ciclo de vida do ANS:

| Módulo | Descrição das Funcionalidades |
| :--- | :--- |
| **Gestão de Acordos** | Inclusão de novos ANS (status inicial pendente), edição de dados e exclusão lógica (inativação) de acordos não assinados. |
| **Consulta e Pesquisa** | Listagem completa com ordenação (coparticipada, vigência) e filtros avançados (palavras-chave, situação, pendências). Visualização detalhada de todas as informações do ANS. |
| **Assinaturas e Aprovações** | Fluxo para chefes de unidades assinarem os acordos. O ANS torna-se "Ativo" apenas após as duas assinaturas. |
| **Governança (Dupla-Custódia)** | Fluxo exclusivo para a GEGOD: solicitação de inativação precoce com justificativa e aprovação/recusa por empregado com função gerencial. |
| **Auditoria e Histórico** | Registro automático de quem criou, editou, assinou ou inativou um ANS, com carimbo de tempo. |

## 6. Fluxos de Negócio do ANS

O ciclo de vida de um Acordo de Nível de Serviço segue estados bem definidos:

1.  **Criação**: Um empregado da GEGOD cadastra o ANS. O sistema o salva com a situação `Pendente`.
2.  **Assinatura**: Os responsáveis pelas unidades fornecedora e consumidora acessam o sistema e assinam digitalmente.
3.  **Ativação**: Quando a segunda assinatura é registrada, o sistema altera automaticamente a situação para `Ativo`.
4.  **Vencimento**: Se a data atual ultrapassar a data de vigência calculada, o sistema pode inativar o ANS automaticamente (processo em lote/worker).
5.  **Inativação Precoce (Dupla-Custódia)**:
    - Um empregado GEGOD solicita a inativação, informa a justificativa e indica o gerente aprovador. O ANS muda para `Pendente Inativação`.
    - O gerente indicado acessa o sistema e aprova. O ANS muda para `Inativo`. (Se recusar, volta para `Ativo`).
6.  **Exclusão Lógica**: Um ANS `Pendente` (sem ambas as assinaturas) pode ser excluído por um empregado GEGOD, passando para o estado `Excluído` (oculto nas listagens padrão).

## 7. Perfis de Acesso

A segurança e a visibilidade das funcionalidades são controladas por perfis baseados em funções (Role-Based Access Control - RBAC):

| Perfil | Permissões no Sistema |
| :--- | :--- |
| **Empregado GEGOD** | Incluir ANS, listar todos, filtrar por pendências, editar, solicitar inativação precoce (com justificativa) e excluir ANS pendentes. |
| **Gerente GEGOD** | Todas as permissões do Empregado GEGOD, mais a autorização (ou recusa) de inativações precoces. |
| **Chefe de Unidade** | Consultar ANS onde sua unidade é parte e assinar acordos (como fornecedora ou consumidora). |
| **Visualizador** | Acesso restrito à consulta e detalhamento de ANS ativos (para auditoria ou consulta geral). |

## 8. Regras de Negócio Críticas

O motor de regras (Domain Layer) garante a integridade dos dados:

-   Todo novo ANS nasce obrigatoriamente com o status `Pendente`.
-   Um ANS só pode ser ativado quando possui as assinaturas da Unidade Fornecedora e da Unidade Consumidora.
-   A exclusão de um ANS só é permitida se ele ainda não possuir ambas as assinaturas. A exclusão é lógica (soft delete).
-   A inativação antes do fim da vigência exige preenchimento de justificativa e indicação de um gerente aprovador.
-   O grau de sigilo deve seguir estritamente a normatização MN OR016.

## 9. Modelagem de Dados e Nomenclatura

A modelagem segue rigorosamente o **Guia de Nomenclatura de Objetos** e o **Guia de Expressões Regulares que Definem Objetos Físicos dos SGBD** da CAIXA.

### Padrões Aplicados:
- Tabelas no singular, prefixadas com o subsistema (ex: `TB_ANS_ACORDO`).
- Colunas com prefixos indicativos do tipo de dado (ex: `CO_` para código/chave, `NO_` para nome, `DT_` para data, `IC_` para indicador booleano).
- Chaves primárias (PK) e estrangeiras (FK) nomeadas explicitamente.

### Sugestão de Entidades e Relacionamentos

| Tabela (Entidade) | Descrição | Principais Colunas |
| :--- | :--- | :--- |
| `TB_ANS_ACORDO` | Armazena os dados principais do Acordo de Nível de Serviço. | `CO_ACORDO` (PK), `NO_FORNECEDORA`, `NO_CONSUMIDORA`, `QT_DIAS_VIGENCIA`, `CO_SITUACAO`, `DT_CRIACAO`. |
| `TB_ANS_RESPONSAVEL` | Dados dos responsáveis técnicos e assinantes (chefes de unidade). | `CO_RESPONSAVEL` (PK), `CO_ACORDO` (FK), `NO_PESSOA`, `NU_CPF`, `NU_MATRICULA`, `TP_PAPEL` (Fornecedor/Consumidor/Técnico). |
| `TB_ANS_DADO_COMPARTILHADO` | Detalha os objetos físicos e lógicos compartilhados no acordo. | `CO_DADO` (PK), `CO_ACORDO` (FK), `NO_NEGOCIAL`, `NO_OBJETO_FISICO`, `DE_LINK`, `IC_DADO_PESSOAL`, `IC_DADO_SENSIVEL`. |
| `TB_ANS_HISTORICO` | Trilha de auditoria para registrar mudanças de status e inativações. | `CO_HISTORICO` (PK), `CO_ACORDO` (FK), `CO_SITUACAO_ANTERIOR`, `CO_SITUACAO_NOVA`, `DE_JUSTIFICATIVA`, `DT_ALTERACAO`, `NU_MATRICULA_RESPONSAVEL`. |

## 10. Endpoints da API REST

A comunicação entre o front-end e o back-end ocorre via API RESTful, utilizando os verbos HTTP adequados:

-   `GET /api/v1/acordos`: Lista os acordos (com parâmetros de paginação, ordenação e filtros).
-   `GET /api/v1/acordos/{id}`: Retorna os detalhes completos de um ANS específico.
-   `POST /api/v1/acordos`: Cria um novo ANS (status pendente).
-   `PUT /api/v1/acordos/{id}`: Atualiza os dados de um ANS existente.
-   `PATCH /api/v1/acordos/{id}/assinar`: Registra a assinatura de uma das partes.
-   `POST /api/v1/acordos/{id}/solicitar-inativacao`: Inicia o fluxo de dupla-custódia.
-   `PATCH /api/v1/acordos/{id}/avaliar-inativacao`: Gerente aprova ou recusa a inativação.
-   `DELETE /api/v1/acordos/{id}`: Realiza a exclusão lógica do ANS.

## 11. Estratégia de Autenticação e Autorização

A solução integrará com o sistema de identidade corporativo da CAIXA. Para o escopo do desafio, a API implementa autenticação baseada em **Tokens JWT (JSON Web Tokens)**.

O token JWT contém as *claims* (declarações) do usuário, como Matrícula, Nome e Perfis de Acesso (Roles). A autorização é garantida por atributos no C# (`[Authorize(Roles = "GEGOD")]`) e *guards* de rotas no Angular, impedindo acessos indevidos.

## 12. Validações e Tratamento de Erros

A resiliência do sistema é garantida por validações em múltiplas camadas:

-   **Front-end (Angular)**: Formulários reativos (Reactive Forms) com validações síncronas (campos obrigatórios, formato de CPF/E-mail) e assíncronas, fornecendo feedback visual imediato ao usuário.
-   **Back-end (C#)**: Uso do `FluentValidation` para garantir que dados maliciosos ou incompletos não alcancem o domínio.
-   **Tratamento de Erros Global**: Implementação de um *Middleware de Exception Handling* no ASP.NET Core, que captura exceções não tratadas, registra no log e retorna respostas padronizadas (RFC 7807 - Problem Details) para o front-end, sem expor a stack trace.

## 13. Estrutura de Repositórios e Deploy

O código-fonte será organizado em dois repositórios no Git (Fontes.CAIXA), separando front-end e back-end para facilitar pipelines de CI/CD independentes.

### Estrutura do Back-end (.NET)
```text
/src
  /Caixa.Ans.Domain        (Entidades, Interfaces)
  /Caixa.Ans.Application   (Casos de Uso, DTOs)
  /Caixa.Ans.Infrastructure(EF Core, Repositórios)
  /Caixa.Ans.Api           (Controllers, Configurações)
/tests
  /Caixa.Ans.UnitTests     (Testes com xUnit e Moq)
```

### Estrutura do Front-end (Angular)
```text
/src
  /app
    /core        (Serviços HTTP, Guards, Interceptors)
    /shared      (Componentes DSC, Pipes, Models)
    /features    (Módulos lazy-loaded: Acordos, Dashboard)
```

### Scripts de Execução e Deploy
A entrega inclui scripts `.bat` e `.ps1` (PowerShell) para:
1.  **Execução Local**: Restaura pacotes (`dotnet restore`, `npm install`), aplica migrações do banco de dados SQLite/SQL Server e inicia os servidores de desenvolvimento.
2.  **Deploy IIS**: Compila a aplicação (`dotnet publish`, `ng build --prod`), configura o Application Pool e o Site no IIS (recebendo o nome do site como parâmetro) e ajusta as permissões de pasta.

## 14. Diferenciais Técnicos e Inovações

Para maximizar o desempenho nos critérios de avaliação e entregar valor além do solicitado, a proposta inclui:

-   **Painel Gerencial (Dashboard)**: Uma tela inicial com gráficos (Recharts) mostrando a distribuição de ANS por status, vencimentos próximos e volumetria de dados compartilhados, auxiliando a tomada de decisão da GEGOD.
-   **Notificações Automáticas**: Arquitetura preparada para disparar e-mails corporativos aos gerentes quando uma inativação precoce for solicitada.
-   **Documentação Viva**: Swagger/OpenAPI configurado com exemplos de requisições e respostas, facilitando a integração futura com outros sistemas do conglomerado.
-   **Testes Automatizados**: Cobertura de testes unitários nas regras de negócio críticas (Domain e Application), garantindo manutenibilidade e clareza de código.

## 15. Conclusão Executiva

O Sistema de Gestão de ANS proposto entrega uma solução madura, segura e perfeitamente aderente ao ambiente corporativo da CAIXA. A adoção de Clean Architecture e Angular com o Design System oficial garante não apenas o cumprimento de todos os requisitos do desafio PSI 15191, mas também a construção de um produto de software escalável, de manutenção acessível e preparado para evoluções futuras no ecossistema de Governança de Dados da instituição.

---

> **Documentação completa e painel interativo:** [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org)
> **Portfólio do autor:** [diogograwingholt.com.br](https://www.diogograwingholt.com.br)
> **Repositório:** [github.com/dsolla2017-pixel/ans-gestao-psi15191](https://github.com/dsolla2017-pixel/ans-gestao-psi15191)
>
> © 2025 Diogo Grawingholt — Todos os direitos reservados
