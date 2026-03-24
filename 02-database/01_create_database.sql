-- Referência: https://gegodtransformacaodosdados.org
-- Portfólio: https://www.diogograwingholt.com.br
-- ============================================================
-- Autor: Diogo Grawingholt
-- Projeto: Sistema de Gestão de ANS - CAIXA
-- ============================================================
-- REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
-- Este script cria o banco de dados e todas as tabelas necessarias
-- para o Sistema de Gestão de ANS. Execute no SQL Server Management
-- Studio (SSMS) ou via sqlcmd.
-- ============================================================

-- Criar banco de dados
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AnsGestao')
BEGIN
    CREATE DATABASE AnsGestao
    COLLATE Latin1_General_CI_AI;
END
GO

USE AnsGestao;
GO

-- ============================================================
-- TABELA: Acordos
-- Armazena os Acordos de Nivel de Servico entre a CAIXA e coparticipadas.
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Acordos')
BEGIN
    CREATE TABLE Acordos (
        CoAcordo            INT IDENTITY(1,1) PRIMARY KEY,
        NoFornecedora       NVARCHAR(200)   NOT NULL,
        NoConsumidora       NVARCHAR(200)   NOT NULL,
        NoUnidadeFornecedora NVARCHAR(200)  NOT NULL,
        NoUnidadeConsumidora NVARCHAR(200)  NOT NULL,
        QtDiasVigencia      INT             NOT NULL DEFAULT 365,
        CoGrauSigilo        NVARCHAR(50)    NOT NULL DEFAULT 'Interno',
        DePeriodicidade     NVARCHAR(100)   NOT NULL DEFAULT 'Mensal',
        IcDadoPessoal       BIT             NOT NULL DEFAULT 0,
        IcDadoSensivel      BIT             NOT NULL DEFAULT 0,
        CoSituacao          NVARCHAR(30)    NOT NULL DEFAULT 'Pendente',
        DeJustificativaInativacao NVARCHAR(500) NULL,
        DtCriacao           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        DtAssinatura        DATETIME2       NULL,
        DtFimVigencia       DATETIME2       NULL,
        DtInativacao        DATETIME2       NULL,
        IcExcluido          BIT             NOT NULL DEFAULT 0,

        CONSTRAINT CK_Acordos_Situacao CHECK (
            CoSituacao IN ('Pendente', 'Ativo', 'Inativo', 'PendenteInativacao')
        ),
        CONSTRAINT CK_Acordos_GrauSigilo CHECK (
            CoGrauSigilo IN ('Publico', 'Interno', 'Confidencial', 'Restrito', 'Secreto')
        )
    );

    CREATE INDEX IX_Acordos_Situacao ON Acordos(CoSituacao);
    CREATE INDEX IX_Acordos_Fornecedora ON Acordos(NoFornecedora);
    CREATE INDEX IX_Acordos_Consumidora ON Acordos(NoConsumidora);
    CREATE INDEX IX_Acordos_DtCriacao ON Acordos(DtCriacao DESC);
END
GO

-- ============================================================
-- TABELA: Responsaveis
-- Armazena os responsaveis tecnicos e assinantes de cada acordo.
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Responsaveis')
BEGIN
    CREATE TABLE Responsaveis (
        CoResponsavel       INT IDENTITY(1,1) PRIMARY KEY,
        CoAcordo            INT             NOT NULL,
        NoPessoa            NVARCHAR(200)   NOT NULL,
        NuCpf               NVARCHAR(11)    NOT NULL,
        NuMatricula         NVARCHAR(20)    NULL,
        NuTelefone          NVARCHAR(20)    NULL,
        DeEmail             NVARCHAR(200)   NULL,
        NoCargoFuncao       NVARCHAR(200)   NULL,
        TpPapel             NVARCHAR(30)    NOT NULL,
        DtAssinatura        DATETIME2       NULL,

        CONSTRAINT FK_Responsaveis_Acordo FOREIGN KEY (CoAcordo)
            REFERENCES Acordos(CoAcordo) ON DELETE CASCADE,
        CONSTRAINT CK_Responsaveis_Papel CHECK (
            TpPapel IN (
                'TecnicoFornecedora', 'TecnicoConsumidora',
                'AssinanteFornecedora', 'AssinanteConsumidora'
            )
        )
    );

    CREATE INDEX IX_Responsaveis_Acordo ON Responsaveis(CoAcordo);
END
GO

-- ============================================================
-- TABELA: DadosCompartilhados
-- Armazena os dados que sao objeto de compartilhamento no acordo.
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DadosCompartilhados')
BEGIN
    CREATE TABLE DadosCompartilhados (
        CoDado              INT IDENTITY(1,1) PRIMARY KEY,
        CoAcordo            INT             NOT NULL,
        NoNegocial          NVARCHAR(300)   NOT NULL,
        NoObjetoFisico      NVARCHAR(300)   NOT NULL,
        DeLink              NVARCHAR(500)   NULL,
        IcDadoPessoal       BIT             NOT NULL DEFAULT 0,
        IcDadoSensivel      BIT             NOT NULL DEFAULT 0,

        CONSTRAINT FK_DadosCompartilhados_Acordo FOREIGN KEY (CoAcordo)
            REFERENCES Acordos(CoAcordo) ON DELETE CASCADE
    );

    CREATE INDEX IX_DadosCompartilhados_Acordo ON DadosCompartilhados(CoAcordo);
END
GO

-- ============================================================
-- TABELA: HistoricoAcordos
-- Registra todas as acoes realizadas sobre cada acordo (auditoria).
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HistoricoAcordos')
BEGIN
    CREATE TABLE HistoricoAcordos (
        CoHistorico         INT IDENTITY(1,1) PRIMARY KEY,
        CoAcordo            INT             NOT NULL,
        DeAcao              NVARCHAR(100)   NOT NULL,
        DeDetalhes          NVARCHAR(500)   NULL,
        NuMatriculaUsuario  NVARCHAR(20)    NULL,
        DtAcao              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_HistoricoAcordos_Acordo FOREIGN KEY (CoAcordo)
            REFERENCES Acordos(CoAcordo) ON DELETE CASCADE
    );

    CREATE INDEX IX_HistoricoAcordos_Acordo ON HistoricoAcordos(CoAcordo);
    CREATE INDEX IX_HistoricoAcordos_DtAcao ON HistoricoAcordos(DtAcao DESC);
END
GO

PRINT '=== Banco de dados AnsGestao criado com sucesso! ===';
GO
