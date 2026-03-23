-- ============================================================
-- Autor: Diogo Grawingholt
-- Projeto: Sistema de Gestao de ANS - CAIXA
-- ============================================================
-- GLOSSARIO PARA LEIGO:
-- Este script insere dados de teste (massa de dados) para que
-- o avaliador possa testar a aplicacao sem precisar cadastrar
-- dados manualmente.
-- ============================================================

USE AnsGestao;
GO

-- Limpar dados existentes (se houver)
DELETE FROM HistoricoAcordos;
DELETE FROM DadosCompartilhados;
DELETE FROM Responsaveis;
DELETE FROM Acordos;
GO

-- ============================================================
-- ACORDO 1: Ativo (ambas as partes assinaram)
-- ============================================================
INSERT INTO Acordos (NoFornecedora, NoConsumidora, NoUnidadeFornecedora, NoUnidadeConsumidora,
    QtDiasVigencia, CoGrauSigilo, DePeriodicidade, IcDadoPessoal, IcDadoSensivel,
    CoSituacao, DtCriacao, DtAssinatura, DtFimVigencia)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Seguridade',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GESEG - Gerencia de Seguridade',
    365, 'Confidencial', 'Mensal', 1, 0,
    'Ativo', '2025-01-15', '2025-01-20', '2026-01-20'
);

DECLARE @Acordo1 INT = SCOPE_IDENTITY();

INSERT INTO Responsaveis (CoAcordo, NoPessoa, NuCpf, NuMatricula, NuTelefone, DeEmail, TpPapel, DtAssinatura)
VALUES
    (@Acordo1, 'Maria Silva Santos', '12345678901', 'C123456', '(61) 3333-1111', 'maria.santos@caixa.gov.br', 'TecnicoFornecedora', NULL),
    (@Acordo1, 'Carlos Oliveira Lima', '23456789012', 'C234567', '(61) 3333-2222', 'carlos.lima@caixaseg.com.br', 'TecnicoConsumidora', NULL),
    (@Acordo1, 'Ana Paula Ferreira', '34567890123', 'C345678', '(61) 3333-3333', 'ana.ferreira@caixa.gov.br', 'AssinanteFornecedora', '2025-01-18'),
    (@Acordo1, 'Roberto Almeida Costa', '45678901234', NULL, '(11) 4444-4444', 'roberto.costa@caixaseg.com.br', 'AssinanteConsumidora', '2025-01-20');

INSERT INTO DadosCompartilhados (CoAcordo, NoNegocial, NoObjetoFisico, DeLink, IcDadoPessoal, IcDadoSensivel)
VALUES
    (@Acordo1, 'Cadastro de Clientes PF', 'TB_CLIENTE_PF', 'https://dados.caixa/catalogo/tb_cliente_pf', 1, 0),
    (@Acordo1, 'Historico de Sinistros', 'TB_SINISTRO_HIST', NULL, 1, 0);

INSERT INTO HistoricoAcordos (CoAcordo, DeAcao, DeDetalhes, NuMatriculaUsuario, DtAcao)
VALUES
    (@Acordo1, 'Criacao', 'ANS criado com situacao Pendente', 'C123456', '2025-01-15'),
    (@Acordo1, 'Assinatura', 'Assinatura da fornecedora por Ana Paula Ferreira', 'C345678', '2025-01-18'),
    (@Acordo1, 'Assinatura', 'Assinatura da consumidora por Roberto Almeida Costa', NULL, '2025-01-20'),
    (@Acordo1, 'Ativacao', 'ANS ativado automaticamente apos ambas assinaturas', 'SISTEMA', '2025-01-20');

-- ============================================================
-- ACORDO 2: Pendente (aguardando assinatura da consumidora)
-- ============================================================
INSERT INTO Acordos (NoFornecedora, NoConsumidora, NoUnidadeFornecedora, NoUnidadeConsumidora,
    QtDiasVigencia, CoGrauSigilo, DePeriodicidade, IcDadoPessoal, IcDadoSensivel,
    CoSituacao, DtCriacao)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Loterias',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GELOT - Gerencia de Loterias',
    180, 'Interno', 'Semanal', 0, 0,
    'Pendente', '2025-03-01'
);

DECLARE @Acordo2 INT = SCOPE_IDENTITY();

INSERT INTO Responsaveis (CoAcordo, NoPessoa, NuCpf, NuMatricula, NuTelefone, DeEmail, TpPapel, DtAssinatura)
VALUES
    (@Acordo2, 'Joao Pedro Souza', '56789012345', 'C456789', '(61) 3333-5555', 'joao.souza@caixa.gov.br', 'TecnicoFornecedora', NULL),
    (@Acordo2, 'Fernanda Rocha', '67890123456', 'C567890', '(21) 5555-6666', 'fernanda.rocha@caixalot.com.br', 'TecnicoConsumidora', NULL),
    (@Acordo2, 'Paulo Henrique Dias', '78901234567', 'C678901', '(61) 3333-7777', 'paulo.dias@caixa.gov.br', 'AssinanteFornecedora', '2025-03-05'),
    (@Acordo2, 'Lucia Mendes', '89012345678', NULL, '(21) 5555-8888', 'lucia.mendes@caixalot.com.br', 'AssinanteConsumidora', NULL);

INSERT INTO DadosCompartilhados (CoAcordo, NoNegocial, NoObjetoFisico, DeLink, IcDadoPessoal, IcDadoSensivel)
VALUES
    (@Acordo2, 'Resultados de Sorteios', 'TB_SORTEIO_RESULTADO', 'https://dados.caixa/catalogo/tb_sorteio', 0, 0);

INSERT INTO HistoricoAcordos (CoAcordo, DeAcao, DeDetalhes, NuMatriculaUsuario, DtAcao)
VALUES
    (@Acordo2, 'Criacao', 'ANS criado com situacao Pendente', 'C456789', '2025-03-01'),
    (@Acordo2, 'Assinatura', 'Assinatura da fornecedora por Paulo Henrique Dias', 'C678901', '2025-03-05');

-- ============================================================
-- ACORDO 3: Pendente Inativacao (solicitacao de inativacao precoce)
-- ============================================================
INSERT INTO Acordos (NoFornecedora, NoConsumidora, NoUnidadeFornecedora, NoUnidadeConsumidora,
    QtDiasVigencia, CoGrauSigilo, DePeriodicidade, IcDadoPessoal, IcDadoSensivel,
    CoSituacao, DeJustificativaInativacao, DtCriacao, DtAssinatura, DtFimVigencia)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Cartoes',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GECAR - Gerencia de Cartoes',
    730, 'Restrito', 'Diaria', 1, 1,
    'PendenteInativacao', 'Reestruturacao societaria da CAIXA Cartoes. Dados serao migrados para novo sistema.',
    '2024-06-01', '2024-06-10', '2026-06-10'
);

DECLARE @Acordo3 INT = SCOPE_IDENTITY();

INSERT INTO Responsaveis (CoAcordo, NoPessoa, NuCpf, NuMatricula, NuTelefone, DeEmail, TpPapel, DtAssinatura)
VALUES
    (@Acordo3, 'Ricardo Gomes', '90123456789', 'C789012', '(61) 3333-9999', 'ricardo.gomes@caixa.gov.br', 'TecnicoFornecedora', NULL),
    (@Acordo3, 'Beatriz Nunes', '01234567890', NULL, '(11) 6666-0000', 'beatriz.nunes@caixacart.com.br', 'TecnicoConsumidora', NULL),
    (@Acordo3, 'Marcos Pereira', '11223344556', 'C890123', '(61) 3333-1122', 'marcos.pereira@caixa.gov.br', 'AssinanteFornecedora', '2024-06-08'),
    (@Acordo3, 'Juliana Martins', '22334455667', NULL, '(11) 6666-2233', 'juliana.martins@caixacart.com.br', 'AssinanteConsumidora', '2024-06-10');

INSERT INTO DadosCompartilhados (CoAcordo, NoNegocial, NoObjetoFisico, DeLink, IcDadoPessoal, IcDadoSensivel)
VALUES
    (@Acordo3, 'Dados de Transacoes de Cartao', 'TB_TRANSACAO_CARTAO', NULL, 1, 1),
    (@Acordo3, 'Limite de Credito por Cliente', 'TB_LIMITE_CREDITO', NULL, 1, 0),
    (@Acordo3, 'Score de Risco', 'VW_SCORE_RISCO', 'https://dados.caixa/catalogo/vw_score_risco', 1, 1);

INSERT INTO HistoricoAcordos (CoAcordo, DeAcao, DeDetalhes, NuMatriculaUsuario, DtAcao)
VALUES
    (@Acordo3, 'Criacao', 'ANS criado com situacao Pendente', 'C789012', '2024-06-01'),
    (@Acordo3, 'Assinatura', 'Assinatura da fornecedora', 'C890123', '2024-06-08'),
    (@Acordo3, 'Assinatura', 'Assinatura da consumidora', NULL, '2024-06-10'),
    (@Acordo3, 'Ativacao', 'ANS ativado', 'SISTEMA', '2024-06-10'),
    (@Acordo3, 'SolicitacaoInativacao', 'Solicitacao de inativacao precoce com justificativa', 'C789012', '2025-02-15');

-- ============================================================
-- ACORDO 4: Inativo (ja inativado)
-- ============================================================
INSERT INTO Acordos (NoFornecedora, NoConsumidora, NoUnidadeFornecedora, NoUnidadeConsumidora,
    QtDiasVigencia, CoGrauSigilo, DePeriodicidade, IcDadoPessoal, IcDadoSensivel,
    CoSituacao, DeJustificativaInativacao, DtCriacao, DtAssinatura, DtFimVigencia, DtInativacao)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Vida e Previdencia',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GEVIP - Gerencia de Vida e Previdencia',
    365, 'Confidencial', 'Trimestral', 1, 0,
    'Inativo', 'Contrato de prestacao de servicos encerrado.',
    '2023-01-10', '2023-01-15', '2024-01-15', '2024-01-15'
);

DECLARE @Acordo4 INT = SCOPE_IDENTITY();

INSERT INTO Responsaveis (CoAcordo, NoPessoa, NuCpf, NuMatricula, NuTelefone, DeEmail, TpPapel, DtAssinatura)
VALUES
    (@Acordo4, 'Thiago Barbosa', '33445566778', 'C901234', '(61) 3333-3344', 'thiago.barbosa@caixa.gov.br', 'TecnicoFornecedora', NULL),
    (@Acordo4, 'Camila Araujo', '44556677889', NULL, '(11) 7777-4455', 'camila.araujo@caixavip.com.br', 'TecnicoConsumidora', NULL);

INSERT INTO DadosCompartilhados (CoAcordo, NoNegocial, NoObjetoFisico, DeLink, IcDadoPessoal, IcDadoSensivel)
VALUES
    (@Acordo4, 'Dados de Aposentadoria', 'TB_APOSENTADORIA', NULL, 1, 0);

-- ============================================================
-- ACORDO 5: Pendente (recém criado, sem assinaturas)
-- ============================================================
INSERT INTO Acordos (NoFornecedora, NoConsumidora, NoUnidadeFornecedora, NoUnidadeConsumidora,
    QtDiasVigencia, CoGrauSigilo, DePeriodicidade, IcDadoPessoal, IcDadoSensivel,
    CoSituacao, DtCriacao)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Consorcio',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GECON - Gerencia de Consorcio',
    540, 'Publico', 'Sob demanda', 0, 0,
    'Pendente', GETUTCDATE()
);

DECLARE @Acordo5 INT = SCOPE_IDENTITY();

INSERT INTO Responsaveis (CoAcordo, NoPessoa, NuCpf, NuMatricula, NuTelefone, DeEmail, TpPapel)
VALUES
    (@Acordo5, 'Amanda Ribeiro', '55667788990', 'C012345', '(61) 3333-5566', 'amanda.ribeiro@caixa.gov.br', 'TecnicoFornecedora'),
    (@Acordo5, 'Diego Fernandes', '66778899001', NULL, '(21) 8888-6677', 'diego.fernandes@caixacon.com.br', 'TecnicoConsumidora');

INSERT INTO DadosCompartilhados (CoAcordo, NoNegocial, NoObjetoFisico, DeLink, IcDadoPessoal, IcDadoSensivel)
VALUES
    (@Acordo5, 'Cotas de Consorcio', 'TB_COTA_CONSORCIO', 'https://dados.caixa/catalogo/tb_cota_consorcio', 0, 0),
    (@Acordo5, 'Contemplacoes', 'TB_CONTEMPLACAO', NULL, 0, 0);

PRINT '=== Massa de dados inserida com sucesso! (5 acordos) ===';
GO
