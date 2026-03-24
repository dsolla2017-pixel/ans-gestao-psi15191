-- ============================================================
-- Autor: Diogo Grawingholt
-- Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
-- Referência: https://gegodtransformacaodosdados.org
-- Portfólio: https://www.diogograwingholt.com.br
-- ============================================================
-- REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
--
-- [Nomenclatura CAIXA] Todas as tabelas seguem o padrão corporativo
--   com prefixo TB_ANS_ e colunas em UPPER_SNAKE_CASE.
--
-- [EF Core Alignment] Este script é a fonte de verdade para produção
--   (SQL Server). O mapeamento Fluent API no AnsDbContext.cs espelha
--   exatamente esta estrutura, garantindo zero divergência.
--
-- [Idempotência] Todos os CREATE utilizam IF NOT EXISTS para permitir
--   reexecução segura do script sem perda de dados.
--
-- [LGPD by Design] Campos de dados pessoais (CPF, nome, email) são
--   identificados com comentários para facilitar auditoria LGPD.
-- ============================================================

-- Criar banco de dados (idempotente)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AnsGestao')
BEGIN
    CREATE DATABASE AnsGestao
    COLLATE Latin1_General_CI_AI;
END
GO

USE AnsGestao;
GO

-- ============================================================
-- TABELA: TB_ANS_ACORDO (Aggregate Root)
-- Armazena os Acordos de Nível de Serviço entre a CAIXA e
-- as empresas coparticipadas do Conglomerado.
-- Mapeamento EF Core: AnsDbContext.cs → modelBuilder.Entity<Acordo>
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TB_ANS_ACORDO')
BEGIN
    CREATE TABLE TB_ANS_ACORDO (
        CO_ACORDO                       INT IDENTITY(1,1) PRIMARY KEY,
        NO_FORNECEDORA                  NVARCHAR(200)   NOT NULL,
        NO_CONSUMIDORA                  NVARCHAR(200)   NOT NULL,
        NO_UNIDADE_FORNECEDORA          NVARCHAR(200)   NOT NULL,
        NO_UNIDADE_CONSUMIDORA          NVARCHAR(200)   NOT NULL,
        QT_DIAS_VIGENCIA                INT             NOT NULL DEFAULT 365,
        CO_GRAU_SIGILO                  NVARCHAR(50)    NOT NULL DEFAULT 'Interno',
        DE_PERIODICIDADE                NVARCHAR(100)   NOT NULL DEFAULT 'Mensal',
        IC_DADO_PESSOAL                 BIT             NOT NULL DEFAULT 0,
        IC_DADO_SENSIVEL                BIT             NOT NULL DEFAULT 0,
        -- CO_SITUACAO armazena o valor numérico do enum SituacaoAcordo (C#)
        -- 1=Pendente, 2=Ativo, 3=Inativo, 4=Excluido, 5=PendenteInativacao
        CO_SITUACAO                     INT             NOT NULL DEFAULT 1,
        DE_JUSTIFICATIVA_INATIVACAO     NVARCHAR(500)   NULL,
        NU_MATRICULA_GERENTE_APROVADOR  NVARCHAR(20)    NULL,
        NU_MATRICULA_CRIADOR            NVARCHAR(20)    NOT NULL DEFAULT '',
        DT_CRIACAO                      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        DT_ASSINATURA                   DATETIME2       NULL,
        DT_FIM_VIGENCIA                 DATETIME2       NULL,
        DT_ULTIMA_ALTERACAO             DATETIME2       NULL,

        CONSTRAINT CK_ANS_ACORDO_SITUACAO CHECK (CO_SITUACAO IN (1, 2, 3, 4, 5)),
        CONSTRAINT CK_ANS_ACORDO_GRAU_SIGILO CHECK (
            CO_GRAU_SIGILO IN ('Publico', 'Interno', 'Confidencial', 'Restrito', 'Secreto')
        )
    );

    CREATE INDEX IX_ANS_ACORDO_SITUACAO ON TB_ANS_ACORDO(CO_SITUACAO);
    CREATE INDEX IX_ANS_ACORDO_FORNECEDORA ON TB_ANS_ACORDO(NO_FORNECEDORA);
    CREATE INDEX IX_ANS_ACORDO_CONSUMIDORA ON TB_ANS_ACORDO(NO_CONSUMIDORA);
    CREATE INDEX IX_ANS_ACORDO_DT_CRIACAO ON TB_ANS_ACORDO(DT_CRIACAO DESC);
END
GO

-- ============================================================
-- TABELA: TB_ANS_RESPONSAVEL
-- Armazena responsáveis técnicos e assinantes de cada acordo.
-- Campos LGPD: NO_PESSOA, NU_CPF, DE_EMAIL
-- Mapeamento EF Core: AnsDbContext.cs → modelBuilder.Entity<Responsavel>
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TB_ANS_RESPONSAVEL')
BEGIN
    CREATE TABLE TB_ANS_RESPONSAVEL (
        CO_RESPONSAVEL      INT IDENTITY(1,1) PRIMARY KEY,
        CO_ACORDO            INT             NOT NULL,
        NO_PESSOA            NVARCHAR(200)   NOT NULL,       -- LGPD: dado pessoal
        NU_CPF               NVARCHAR(11)    NOT NULL,       -- LGPD: dado pessoal
        NU_MATRICULA         NVARCHAR(20)    NULL,
        NU_TELEFONE          NVARCHAR(20)    NULL,
        DE_EMAIL             NVARCHAR(200)   NULL,           -- LGPD: dado pessoal
        NO_CARGO_FUNCAO      NVARCHAR(100)   NULL,
        -- TP_PAPEL armazena o valor numérico do enum TipoPapel (C#)
        -- 1=TecnicoFornecedora, 2=TecnicoConsumidora,
        -- 3=AssinanteFornecedora, 4=AssinanteConsumidora
        TP_PAPEL             INT             NOT NULL,
        DT_ASSINATURA        DATETIME2       NULL,

        CONSTRAINT FK_ANS_RESPONSAVEL_ACORDO FOREIGN KEY (CO_ACORDO)
            REFERENCES TB_ANS_ACORDO(CO_ACORDO) ON DELETE CASCADE,
        CONSTRAINT CK_ANS_RESPONSAVEL_PAPEL CHECK (TP_PAPEL IN (1, 2, 3, 4))
    );

    CREATE INDEX IX_ANS_RESPONSAVEL_ACORDO ON TB_ANS_RESPONSAVEL(CO_ACORDO);
END
GO

-- ============================================================
-- TABELA: TB_ANS_DADO_COMPARTILHADO
-- Armazena os ativos de informação objeto do compartilhamento.
-- Mapeamento EF Core: AnsDbContext.cs → modelBuilder.Entity<DadoCompartilhado>
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TB_ANS_DADO_COMPARTILHADO')
BEGIN
    CREATE TABLE TB_ANS_DADO_COMPARTILHADO (
        CO_DADO              INT IDENTITY(1,1) PRIMARY KEY,
        CO_ACORDO            INT             NOT NULL,
        NO_NEGOCIAL          NVARCHAR(200)   NOT NULL,
        NO_OBJETO_FISICO     NVARCHAR(200)   NOT NULL,
        DE_LINK              NVARCHAR(500)   NULL,
        IC_DADO_PESSOAL      BIT             NOT NULL DEFAULT 0,
        IC_DADO_SENSIVEL     BIT             NOT NULL DEFAULT 0,

        CONSTRAINT FK_ANS_DADO_COMPARTILHADO_ACORDO FOREIGN KEY (CO_ACORDO)
            REFERENCES TB_ANS_ACORDO(CO_ACORDO) ON DELETE CASCADE
    );

    CREATE INDEX IX_ANS_DADO_COMPARTILHADO_ACORDO ON TB_ANS_DADO_COMPARTILHADO(CO_ACORDO);
END
GO

-- ============================================================
-- TABELA: TB_ANS_HISTORICO
-- Trilha de auditoria (append-only) para rastreabilidade.
-- Registra transições de estado com situação anterior e nova.
-- Mapeamento EF Core: AnsDbContext.cs → modelBuilder.Entity<HistoricoAcordo>
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TB_ANS_HISTORICO')
BEGIN
    CREATE TABLE TB_ANS_HISTORICO (
        CO_HISTORICO                INT IDENTITY(1,1) PRIMARY KEY,
        CO_ACORDO                   INT             NOT NULL,
        CO_SITUACAO_ANTERIOR        INT             NULL,
        CO_SITUACAO_NOVA            INT             NOT NULL,
        DE_JUSTIFICATIVA            NVARCHAR(1000)  NULL,
        DT_ALTERACAO                DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        NU_MATRICULA_RESPONSAVEL    NVARCHAR(20)    NULL,

        CONSTRAINT FK_ANS_HISTORICO_ACORDO FOREIGN KEY (CO_ACORDO)
            REFERENCES TB_ANS_ACORDO(CO_ACORDO) ON DELETE CASCADE
    );

    CREATE INDEX IX_ANS_HISTORICO_ACORDO ON TB_ANS_HISTORICO(CO_ACORDO);
    CREATE INDEX IX_ANS_HISTORICO_DT_ALTERACAO ON TB_ANS_HISTORICO(DT_ALTERACAO DESC);
END
GO

PRINT '=== Banco de dados AnsGestao criado com sucesso! ===';
PRINT '=== Tabelas: TB_ANS_ACORDO, TB_ANS_RESPONSAVEL, TB_ANS_DADO_COMPARTILHADO, TB_ANS_HISTORICO ===';
PRINT '=== Schema 100%% alinhado com EF Core (AnsDbContext.cs) ===';
GO
