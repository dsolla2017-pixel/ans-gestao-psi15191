// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - DTO (Data Transfer Object): Objeto simplificado para trafegar dados
//   entre o front-end e o back-end, sem expor a estrutura interna do banco.
// ============================================================

using System;
using System.Collections.Generic;

namespace Caixa.Ans.Application.DTOs
{
    /// <summary>
    /// DTO para criacao de um novo ANS.
    /// Recebe os dados do formulario do front-end.
    /// </summary>
    public class CriarAcordoDto
    {
        public string NoFornecedora { get; set; } = string.Empty;
        public string NoConsumidora { get; set; } = string.Empty;
        public string NoUnidadeFornecedora { get; set; } = string.Empty;
        public string NoUnidadeConsumidora { get; set; } = string.Empty;
        public int QtDiasVigencia { get; set; }
        public string CoGrauSigilo { get; set; } = string.Empty;
        public string DePeriodicidade { get; set; } = string.Empty;
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }

        // Responsaveis tecnicos
        public ResponsavelDto ResponsavelFornecedora { get; set; } = new();
        public ResponsavelDto ResponsavelConsumidora { get; set; } = new();

        // Dados compartilhados
        public List<DadoCompartilhadoDto> DadosCompartilhados { get; set; } = new();
    }

    /// <summary>
    /// DTO para exibicao de um ANS na listagem e detalhamento.
    /// </summary>
    public class AcordoResumoDto
    {
        public int CoAcordo { get; set; }
        public string NoFornecedora { get; set; } = string.Empty;
        public string NoConsumidora { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
        public DateTime DtCriacao { get; set; }
        public DateTime? DtAssinatura { get; set; }
        public DateTime? DtFimVigencia { get; set; }
        public int QtDiasVigencia { get; set; }
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }
    }

    /// <summary>
    /// DTO completo para detalhamento de um ANS.
    /// </summary>
    public class AcordoDetalheDto : AcordoResumoDto
    {
        public string NoUnidadeFornecedora { get; set; } = string.Empty;
        public string NoUnidadeConsumidora { get; set; } = string.Empty;
        public string CoGrauSigilo { get; set; } = string.Empty;
        public string DePeriodicidade { get; set; } = string.Empty;
        public string? DeJustificativaInativacao { get; set; }

        public List<ResponsavelDto> Responsaveis { get; set; } = new();
        public List<DadoCompartilhadoDto> DadosCompartilhados { get; set; } = new();
    }

    /// <summary>
    /// DTO para dados de um responsavel (tecnico ou assinante).
    /// </summary>
    public class ResponsavelDto
    {
        public string NoPessoa { get; set; } = string.Empty;
        public string NuCpf { get; set; } = string.Empty;
        public string? NuMatricula { get; set; }
        public string? NuTelefone { get; set; }
        public string? DeEmail { get; set; }
        public string? NoCargoFuncao { get; set; }
        public string TpPapel { get; set; } = string.Empty;
        public DateTime? DtAssinatura { get; set; }
    }

    /// <summary>
    /// DTO para dados compartilhados no ANS.
    /// </summary>
    public class DadoCompartilhadoDto
    {
        public string NoNegocial { get; set; } = string.Empty;
        public string NoObjetoFisico { get; set; } = string.Empty;
        public string? DeLink { get; set; }
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }
    }

    /// <summary>
    /// DTO para solicitar inativacao precoce (dupla-custodia).
    /// </summary>
    public class SolicitarInativacaoDto
    {
        public string DeJustificativa { get; set; } = string.Empty;
        public string NuMatriculaGerenteAprovador { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para avaliar (aprovar/recusar) inativacao.
    /// </summary>
    public class AvaliarInativacaoDto
    {
        public bool Aprovado { get; set; }
    }
}
