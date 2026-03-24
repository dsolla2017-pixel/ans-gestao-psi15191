// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [DTO Pattern] Martin Fowler — Patterns of Enterprise Application
//   Architecture: Data Transfer Objects desacoplam a representação
//   externa (API) da estrutura interna do domínio. Isso protege
//   as entidades de exposição indevida e permite evolução
//   independente entre front-end e back-end.
//
// [Anti-Corruption Layer] DDD — Eric Evans:
//   Os DTOs atuam como barreira entre a camada de apresentação
//   e o modelo de domínio, impedindo que conceitos externos
//   contaminem as regras de negócio.
//
// [OWASP API Security] Os DTOs limitam a superfície de ataque
//   ao expor apenas os campos necessários para cada operação,
//   evitando o antipadrão Mass Assignment.
// ============================================================

using System;
using System.Collections.Generic;

namespace Caixa.Ans.Application.DTOs
{
    /// <summary>
    /// DTO de entrada para criação de um novo Acordo de Nível de Serviço.
    /// Recebe os dados do formulário do front-end Angular e os valida
    /// antes de transformá-los em uma entidade de domínio.
    /// 
    /// Campos mapeados conforme requisitos do PSI 15191:
    /// Fornecedora, Consumidora, Vigência, Sigilo, LGPD, Responsáveis e Dados.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class CriarAcordoDto
    {
        // ── Partes Envolvidas ──────────────────────────────────────
        // Identificação das empresas do Conglomerado CAIXA que
        // participam do acordo de compartilhamento de dados.
        public string NoFornecedora { get; set; } = string.Empty;
        public string NoConsumidora { get; set; } = string.Empty;

        // Unidades organizacionais responsáveis dentro de cada empresa.
        public string NoUnidadeFornecedora { get; set; } = string.Empty;
        public string NoUnidadeConsumidora { get; set; } = string.Empty;

        // ── Vigência ───────────────────────────────────────────────
        // Período em dias corridos de validade do ANS.
        public int QtDiasVigencia { get; set; }

        // ── Classificação de Dados ─────────────────────────────────
        // Grau de sigilo conforme MN OR016 e indicadores LGPD.
        public string CoGrauSigilo { get; set; } = string.Empty;
        public string DePeriodicidade { get; set; } = string.Empty;
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }

        // ── Responsáveis Técnicos ──────────────────────────────────
        // Pontos focais operacionais de cada parte do acordo.
        // Inicializados com instância vazia para evitar NullReferenceException.
        public ResponsavelDto ResponsavelFornecedora { get; set; } = new();
        public ResponsavelDto ResponsavelConsumidora { get; set; } = new();

        // ── Dados Compartilhados ───────────────────────────────────
        // Lista de ativos de informação objeto do compartilhamento.
        public List<DadoCompartilhadoDto> DadosCompartilhados { get; set; } = new();
    }

    /// <summary>
    /// DTO de saída para exibição resumida de um ANS na listagem.
    /// Contém apenas os campos necessários para a visão tabular,
    /// otimizando o tráfego de rede e o tempo de renderização.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AcordoResumoDto
    {
        // Identificador único para navegação ao detalhamento.
        public int CoAcordo { get; set; }

        // Partes envolvidas — colunas principais da listagem.
        public string NoFornecedora { get; set; } = string.Empty;
        public string NoConsumidora { get; set; } = string.Empty;

        // Situação atual do ANS (Pendente, Ativo, Inativo).
        public string Situacao { get; set; } = string.Empty;

        // Datas do ciclo de vida para ordenação e filtro.
        public DateTime DtCriacao { get; set; }
        public DateTime? DtAssinatura { get; set; }
        public DateTime? DtFimVigencia { get; set; }
        public int QtDiasVigencia { get; set; }

        // Indicadores LGPD para sinalização visual na listagem.
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }
    }

    /// <summary>
    /// DTO de saída para detalhamento completo de um ANS.
    /// Herda os campos do resumo e adiciona informações detalhadas:
    /// unidades, sigilo, periodicidade, responsáveis e dados compartilhados.
    /// 
    /// Padrão aplicado: Herança de DTO para reutilização de campos
    /// comuns entre listagem e detalhamento (DRY — Don't Repeat Yourself).
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AcordoDetalheDto : AcordoResumoDto
    {
        // Detalhes organizacionais das unidades responsáveis.
        public string NoUnidadeFornecedora { get; set; } = string.Empty;
        public string NoUnidadeConsumidora { get; set; } = string.Empty;

        // Classificação de dados conforme MN OR016.
        public string CoGrauSigilo { get; set; } = string.Empty;
        public string DePeriodicidade { get; set; } = string.Empty;

        // Justificativa de inativação precoce (quando aplicável).
        public string? DeJustificativaInativacao { get; set; }

        // Coleções de entidades filhas para exibição completa.
        public List<ResponsavelDto> Responsaveis { get; set; } = new();
        public List<DadoCompartilhadoDto> DadosCompartilhados { get; set; } = new();
    }

    /// <summary>
    /// DTO para representação de um responsável (técnico ou assinante).
    /// Utilizado tanto na entrada (criação) quanto na saída (detalhamento).
    /// 
    /// Campos conforme requisito do PSI 15191: "nome, CPF, matrícula
    /// (se empregado), telefone de contato e e-mail de contato".
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class ResponsavelDto
    {
        // Dados de identificação do responsável.
        public string NoPessoa { get; set; } = string.Empty;
        public string NuCpf { get; set; } = string.Empty;
        public string? NuMatricula { get; set; }
        public string? NuTelefone { get; set; }
        public string? DeEmail { get; set; }
        public string? NoCargoFuncao { get; set; }

        // Papel no acordo (TecnicoFornecedora, AssinanteConsumidora, etc.).
        public string TpPapel { get; set; } = string.Empty;

        // Data da assinatura — preenchida quando o chefe de unidade assina.
        public DateTime? DtAssinatura { get; set; }
    }

    /// <summary>
    /// DTO para representação de um dado compartilhado no ANS.
    /// Descreve o ativo de informação que transita entre as partes,
    /// incluindo sua classificação LGPD.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class DadoCompartilhadoDto
    {
        // Nome negocial e objeto físico que contém o dado.
        public string NoNegocial { get; set; } = string.Empty;
        public string NoObjetoFisico { get; set; } = string.Empty;
        public string? DeLink { get; set; }

        // Classificação LGPD no nível do dado individual.
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }
    }

    /// <summary>
    /// DTO de entrada para solicitação de inativação precoce.
    /// Implementa o primeiro passo do fluxo de dupla-custódia:
    /// o empregado GEGOD fornece a justificativa e indica o gestor aprovador.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class SolicitarInativacaoDto
    {
        // Justificativa obrigatória para a inativação precoce.
        public string DeJustificativa { get; set; } = string.Empty;

        // Matrícula do gestor com função gerencial que deve autorizar.
        public string NuMatriculaGerenteAprovador { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de entrada para avaliação (autorização ou recusa) da
    /// inativação precoce. Utilizado pelo gestor com função gerencial
    /// no segundo passo do fluxo de dupla-custódia.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AvaliarInativacaoDto
    {
        // True = autoriza a inativação; False = recusa a solicitação.
        public bool Aprovado { get; set; }
    }
}
