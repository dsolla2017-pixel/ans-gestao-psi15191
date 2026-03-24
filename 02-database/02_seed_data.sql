-- ============================================================
-- Autor: Diogo Grawingholt
-- Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
-- Referência: https://gegodtransformacaodosdados.org
-- Portfólio: https://www.diogograwingholt.com.br
-- ============================================================
-- REFERÊNCIA TÉCNICA — Massa de Dados para Teste
--
-- Este script insere 5 acordos representativos de todos os estados
-- da máquina de estados do ANS:
--   Acordo 1: Ativo (ambas as partes assinaram)
--   Acordo 2: Pendente (aguarda assinaturas)
--   Acordo 3: PendenteInativacao (solicitação de inativação precoce)
--   Acordo 4: Inativo (encerrado)
--   Acordo 5: Pendente (recém criado)
--
-- Valores numéricos dos enums (alinhados com C#):
--   SituacaoAcordo: 1=Pendente, 2=Ativo, 3=Inativo, 4=Excluido, 5=PendenteInativacao
--   TipoPapel: 1=TecnicoFornecedora, 2=TecnicoConsumidora, 3=AssinanteFornecedora, 4=AssinanteConsumidora
-- ============================================================

USE AnsGestao;
GO

-- Limpar dados existentes (ordem respeitando FKs via CASCADE)
DELETE FROM TB_ANS_HISTORICO;
DELETE FROM TB_ANS_DADO_COMPARTILHADO;
DELETE FROM TB_ANS_RESPONSAVEL;
DELETE FROM TB_ANS_ACORDO;
GO

-- Resetar identities para facilitar referências
DBCC CHECKIDENT ('TB_ANS_ACORDO', RESEED, 0);
DBCC CHECKIDENT ('TB_ANS_RESPONSAVEL', RESEED, 0);
DBCC CHECKIDENT ('TB_ANS_DADO_COMPARTILHADO', RESEED, 0);
DBCC CHECKIDENT ('TB_ANS_HISTORICO', RESEED, 0);
GO

-- ============================================================
-- ACORDO 1: Ativo (ambas as partes assinaram)
-- Cenário: compartilhamento de dados cadastrais com a CAIXA Seguridade
-- ============================================================
INSERT INTO TB_ANS_ACORDO (
    NO_FORNECEDORA, NO_CONSUMIDORA, NO_UNIDADE_FORNECEDORA, NO_UNIDADE_CONSUMIDORA,
    QT_DIAS_VIGENCIA, CO_GRAU_SIGILO, DE_PERIODICIDADE, IC_DADO_PESSOAL, IC_DADO_SENSIVEL,
    CO_SITUACAO, NU_MATRICULA_CRIADOR, DT_CRIACAO, DT_ASSINATURA, DT_FIM_VIGENCIA)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Seguridade',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GESEG - Gerencia de Seguridade',
    365, 'Confidencial', 'Mensal', 1, 0,
    2, 'C123456', '2024-01-15', '2024-01-20', '2025-01-20');
DECLARE @Acordo1 INT = SCOPE_IDENTITY();

-- Responsáveis técnicos e assinantes do Acordo 1
INSERT INTO TB_ANS_RESPONSAVEL (CO_ACORDO, NO_PESSOA, NU_CPF, NU_MATRICULA, NU_TELEFONE, DE_EMAIL, NO_CARGO_FUNCAO, TP_PAPEL, DT_ASSINATURA)
VALUES
    (@Acordo1, 'Ana Paula Silva', '12345678901', 'C123456', '(61) 3333-1111', 'ana.silva@caixa.gov.br', NULL, 1, NULL),
    (@Acordo1, 'Carlos Eduardo Santos', '23456789012', NULL, '(11) 5555-2222', 'carlos.santos@caixaseg.com.br', NULL, 2, NULL),
    (@Acordo1, 'Maria Fernanda Costa', '34567890123', 'C234567', '(61) 3333-3333', 'maria.costa@caixa.gov.br', 'Chefe GEGOD', 3, '2024-01-18'),
    (@Acordo1, 'Roberto Lima Filho', '45678901234', NULL, '(11) 5555-4444', 'roberto.lima@caixaseg.com.br', 'Gerente GESEG', 4, '2024-01-20');

-- Dados compartilhados do Acordo 1
INSERT INTO TB_ANS_DADO_COMPARTILHADO (CO_ACORDO, NO_NEGOCIAL, NO_OBJETO_FISICO, DE_LINK, IC_DADO_PESSOAL, IC_DADO_SENSIVEL)
VALUES
    (@Acordo1, 'Cadastro de Clientes PF', 'TB_CLIENTE_PF', 'https://dados.caixa/catalogo/tb_cliente_pf', 1, 0),
    (@Acordo1, 'Dados de Apolice de Seguro', 'TB_APOLICE_SEGURO', 'https://dados.caixa/catalogo/tb_apolice_seguro', 1, 0);

-- Histórico do Acordo 1
INSERT INTO TB_ANS_HISTORICO (CO_ACORDO, CO_SITUACAO_ANTERIOR, CO_SITUACAO_NOVA, DE_JUSTIFICATIVA, DT_ALTERACAO, NU_MATRICULA_RESPONSAVEL)
VALUES
    (@Acordo1, NULL, 1, 'ANS criado com situacao Pendente', '2024-01-15', 'C123456'),
    (@Acordo1, 1, 2, 'ANS ativado apos assinatura bilateral', '2024-01-20', 'SISTEMA');

-- ============================================================
-- ACORDO 2: Pendente (aguarda assinatura da consumidora)
-- Cenário: compartilhamento de dados habitacionais com a CAIXA Loterias
-- ============================================================
INSERT INTO TB_ANS_ACORDO (
    NO_FORNECEDORA, NO_CONSUMIDORA, NO_UNIDADE_FORNECEDORA, NO_UNIDADE_CONSUMIDORA,
    QT_DIAS_VIGENCIA, CO_GRAU_SIGILO, DE_PERIODICIDADE, IC_DADO_PESSOAL, IC_DADO_SENSIVEL,
    CO_SITUACAO, NU_MATRICULA_CRIADOR, DT_CRIACAO)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Loterias',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GELOT - Gerencia de Loterias',
    180, 'Interno', 'Semanal', 0, 0,
    1, 'C345678', '2025-02-01');
DECLARE @Acordo2 INT = SCOPE_IDENTITY();

INSERT INTO TB_ANS_RESPONSAVEL (CO_ACORDO, NO_PESSOA, NU_CPF, NU_MATRICULA, NU_TELEFONE, DE_EMAIL, TP_PAPEL)
VALUES
    (@Acordo2, 'Fernanda Oliveira', '56789012345', 'C345678', '(61) 3333-5555', 'fernanda.oliveira@caixa.gov.br', 1),
    (@Acordo2, 'Paulo Henrique Souza', '67890123456', NULL, '(21) 7777-6666', 'paulo.souza@caixalot.com.br', 2);

INSERT INTO TB_ANS_DADO_COMPARTILHADO (CO_ACORDO, NO_NEGOCIAL, NO_OBJETO_FISICO, DE_LINK, IC_DADO_PESSOAL, IC_DADO_SENSIVEL)
VALUES
    (@Acordo2, 'Resultados de Sorteios', 'TB_SORTEIO_RESULTADO', 'https://dados.caixa/catalogo/tb_sorteio_resultado', 0, 0),
    (@Acordo2, 'Premiacoes por UF', 'VW_PREMIACAO_UF', NULL, 0, 0);

INSERT INTO TB_ANS_HISTORICO (CO_ACORDO, CO_SITUACAO_ANTERIOR, CO_SITUACAO_NOVA, DE_JUSTIFICATIVA, DT_ALTERACAO, NU_MATRICULA_RESPONSAVEL)
VALUES
    (@Acordo2, NULL, 1, 'ANS criado com situacao Pendente', '2025-02-01', 'C345678');

-- ============================================================
-- ACORDO 3: PendenteInativacao (solicitação de inativação precoce)
-- Cenário: compartilhamento de dados de cartões com a CAIXA Cartões
-- ============================================================
INSERT INTO TB_ANS_ACORDO (
    NO_FORNECEDORA, NO_CONSUMIDORA, NO_UNIDADE_FORNECEDORA, NO_UNIDADE_CONSUMIDORA,
    QT_DIAS_VIGENCIA, CO_GRAU_SIGILO, DE_PERIODICIDADE, IC_DADO_PESSOAL, IC_DADO_SENSIVEL,
    CO_SITUACAO, DE_JUSTIFICATIVA_INATIVACAO, NU_MATRICULA_CRIADOR, NU_MATRICULA_GERENTE_APROVADOR,
    DT_CRIACAO, DT_ASSINATURA, DT_FIM_VIGENCIA)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Cartoes',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GECAR - Gerencia de Cartoes',
    730, 'Restrito', 'Diaria', 1, 1,
    5, 'Reestruturacao societaria da CAIXA Cartoes. Dados serao migrados para novo sistema.',
    'C789012', 'C890123',
    '2024-06-01', '2024-06-10', '2026-06-10');
DECLARE @Acordo3 INT = SCOPE_IDENTITY();

INSERT INTO TB_ANS_RESPONSAVEL (CO_ACORDO, NO_PESSOA, NU_CPF, NU_MATRICULA, NU_TELEFONE, DE_EMAIL, NO_CARGO_FUNCAO, TP_PAPEL, DT_ASSINATURA)
VALUES
    (@Acordo3, 'Ricardo Gomes', '90123456789', 'C789012', '(61) 3333-9999', 'ricardo.gomes@caixa.gov.br', NULL, 1, NULL),
    (@Acordo3, 'Beatriz Nunes', '01234567890', NULL, '(11) 6666-0000', 'beatriz.nunes@caixacart.com.br', NULL, 2, NULL),
    (@Acordo3, 'Marcos Pereira', '11223344556', 'C890123', '(61) 3333-1122', 'marcos.pereira@caixa.gov.br', 'Chefe GEGOD', 3, '2024-06-08'),
    (@Acordo3, 'Juliana Martins', '22334455667', NULL, '(11) 6666-2233', 'juliana.martins@caixacart.com.br', 'Gerente GECAR', 4, '2024-06-10');

INSERT INTO TB_ANS_DADO_COMPARTILHADO (CO_ACORDO, NO_NEGOCIAL, NO_OBJETO_FISICO, DE_LINK, IC_DADO_PESSOAL, IC_DADO_SENSIVEL)
VALUES
    (@Acordo3, 'Dados de Transacoes de Cartao', 'TB_TRANSACAO_CARTAO', NULL, 1, 1),
    (@Acordo3, 'Limite de Credito por Cliente', 'TB_LIMITE_CREDITO', NULL, 1, 0),
    (@Acordo3, 'Score de Risco', 'VW_SCORE_RISCO', 'https://dados.caixa/catalogo/vw_score_risco', 1, 1);

INSERT INTO TB_ANS_HISTORICO (CO_ACORDO, CO_SITUACAO_ANTERIOR, CO_SITUACAO_NOVA, DE_JUSTIFICATIVA, DT_ALTERACAO, NU_MATRICULA_RESPONSAVEL)
VALUES
    (@Acordo3, NULL, 1, 'ANS criado com situacao Pendente', '2024-06-01', 'C789012'),
    (@Acordo3, 1, 2, 'ANS ativado apos assinatura bilateral', '2024-06-10', 'SISTEMA'),
    (@Acordo3, 2, 5, 'Solicitacao de inativacao precoce com justificativa', '2025-02-15', 'C789012');

-- ============================================================
-- ACORDO 4: Inativo (encerrado por término de vigência)
-- Cenário: compartilhamento de dados previdenciários com a CAIXA Vida e Previdência
-- ============================================================
INSERT INTO TB_ANS_ACORDO (
    NO_FORNECEDORA, NO_CONSUMIDORA, NO_UNIDADE_FORNECEDORA, NO_UNIDADE_CONSUMIDORA,
    QT_DIAS_VIGENCIA, CO_GRAU_SIGILO, DE_PERIODICIDADE, IC_DADO_PESSOAL, IC_DADO_SENSIVEL,
    CO_SITUACAO, DE_JUSTIFICATIVA_INATIVACAO, NU_MATRICULA_CRIADOR,
    DT_CRIACAO, DT_ASSINATURA, DT_FIM_VIGENCIA, DT_ULTIMA_ALTERACAO)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Vida e Previdencia',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GEVIP - Gerencia de Vida e Previdencia',
    365, 'Confidencial', 'Trimestral', 1, 0,
    3, 'Contrato de prestacao de servicos encerrado.',
    'C901234',
    '2023-01-10', '2023-01-15', '2024-01-15', '2024-01-15');
DECLARE @Acordo4 INT = SCOPE_IDENTITY();

INSERT INTO TB_ANS_RESPONSAVEL (CO_ACORDO, NO_PESSOA, NU_CPF, NU_MATRICULA, NU_TELEFONE, DE_EMAIL, TP_PAPEL)
VALUES
    (@Acordo4, 'Thiago Barbosa', '33445566778', 'C901234', '(61) 3333-3344', 'thiago.barbosa@caixa.gov.br', 1),
    (@Acordo4, 'Camila Araujo', '44556677889', NULL, '(11) 7777-4455', 'camila.araujo@caixavip.com.br', 2);

INSERT INTO TB_ANS_DADO_COMPARTILHADO (CO_ACORDO, NO_NEGOCIAL, NO_OBJETO_FISICO, DE_LINK, IC_DADO_PESSOAL, IC_DADO_SENSIVEL)
VALUES
    (@Acordo4, 'Dados de Aposentadoria', 'TB_APOSENTADORIA', NULL, 1, 0);

INSERT INTO TB_ANS_HISTORICO (CO_ACORDO, CO_SITUACAO_ANTERIOR, CO_SITUACAO_NOVA, DE_JUSTIFICATIVA, DT_ALTERACAO, NU_MATRICULA_RESPONSAVEL)
VALUES
    (@Acordo4, NULL, 1, 'ANS criado com situacao Pendente', '2023-01-10', 'C901234'),
    (@Acordo4, 1, 2, 'ANS ativado apos assinatura bilateral', '2023-01-15', 'SISTEMA'),
    (@Acordo4, 2, 3, 'ANS inativado por encerramento de contrato', '2024-01-15', 'C901234');

-- ============================================================
-- ACORDO 5: Pendente (recém criado, sem assinaturas)
-- Cenário: compartilhamento de dados de consórcio com a CAIXA Consórcio
-- ============================================================
INSERT INTO TB_ANS_ACORDO (
    NO_FORNECEDORA, NO_CONSUMIDORA, NO_UNIDADE_FORNECEDORA, NO_UNIDADE_CONSUMIDORA,
    QT_DIAS_VIGENCIA, CO_GRAU_SIGILO, DE_PERIODICIDADE, IC_DADO_PESSOAL, IC_DADO_SENSIVEL,
    CO_SITUACAO, NU_MATRICULA_CRIADOR, DT_CRIACAO)
VALUES (
    'CAIXA Economica Federal', 'CAIXA Consorcio',
    'GEGOD - Gerencia Nacional de Governanca de Dados', 'GECON - Gerencia de Consorcio',
    540, 'Publico', 'Sob demanda', 0, 0,
    1, 'C012345', GETUTCDATE());
DECLARE @Acordo5 INT = SCOPE_IDENTITY();

INSERT INTO TB_ANS_RESPONSAVEL (CO_ACORDO, NO_PESSOA, NU_CPF, NU_MATRICULA, NU_TELEFONE, DE_EMAIL, TP_PAPEL)
VALUES
    (@Acordo5, 'Amanda Ribeiro', '55667788990', 'C012345', '(61) 3333-5566', 'amanda.ribeiro@caixa.gov.br', 1),
    (@Acordo5, 'Diego Fernandes', '66778899001', NULL, '(21) 8888-6677', 'diego.fernandes@caixacon.com.br', 2);

INSERT INTO TB_ANS_DADO_COMPARTILHADO (CO_ACORDO, NO_NEGOCIAL, NO_OBJETO_FISICO, DE_LINK, IC_DADO_PESSOAL, IC_DADO_SENSIVEL)
VALUES
    (@Acordo5, 'Cotas de Consorcio', 'TB_COTA_CONSORCIO', 'https://dados.caixa/catalogo/tb_cota_consorcio', 0, 0),
    (@Acordo5, 'Contemplacoes', 'TB_CONTEMPLACAO', NULL, 0, 0);

INSERT INTO TB_ANS_HISTORICO (CO_ACORDO, CO_SITUACAO_ANTERIOR, CO_SITUACAO_NOVA, DE_JUSTIFICATIVA, DT_ALTERACAO, NU_MATRICULA_RESPONSAVEL)
VALUES
    (@Acordo5, NULL, 1, 'ANS criado com situacao Pendente', GETUTCDATE(), 'C012345');

PRINT '=== Massa de dados inserida com sucesso! (5 acordos) ===';
PRINT '=== Estados cobertos: Ativo, Pendente, PendenteInativacao, Inativo, Pendente ===';
GO
