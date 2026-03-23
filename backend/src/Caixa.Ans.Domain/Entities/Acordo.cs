// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Entity: Objeto principal do sistema que representa um Acordo de Nivel de Servico.
// - Property: Caracteristica do acordo (nome, data, status etc.).
// - Enum: Lista fixa de opcoes (ex: Pendente, Ativo, Inativo).
// ============================================================

using System;
using System.Collections.Generic;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade principal que representa um Acordo de Nivel de Servico (ANS)
    /// para compartilhamento de dados entre a CAIXA e empresas do conglomerado.
    /// Mapeada para a tabela TB_ANS_ACORDO conforme Guia de Nomenclatura CAIXA.
    /// </summary>
    public class Acordo
    {
        // Chave primaria (CO_ = codigo)
        public int CoAcordo { get; set; }

        // Dados das partes envolvidas
        public string NoFornecedora { get; set; } = string.Empty;
        public string NoConsumidora { get; set; } = string.Empty;
        public string NoUnidadeFornecedora { get; set; } = string.Empty;
        public string NoUnidadeConsumidora { get; set; } = string.Empty;

        // Vigencia
        public int QtDiasVigencia { get; set; }
        public DateTime DtCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DtAssinatura { get; set; }
        public DateTime? DtFimVigencia { get; set; }

        // Situacao do acordo (Pendente, Ativo, Inativo, Excluido, PendenteInativacao)
        public SituacaoAcordo CoSituacao { get; set; } = SituacaoAcordo.Pendente;

        // Classificacao dos dados
        public string CoGrauSigilo { get; set; } = string.Empty;
        public string DePeriodicidade { get; set; } = string.Empty;
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }

        // Inativacao precoce (dupla-custodia)
        public string? DeJustificativaInativacao { get; set; }
        public string? NuMatriculaGerenteAprovador { get; set; }

        // Auditoria
        public string NuMatriculaCriador { get; set; } = string.Empty;
        public DateTime? DtUltimaAlteracao { get; set; }

        // Relacionamentos (navegacao)
        public ICollection<Responsavel> Responsaveis { get; set; } = new List<Responsavel>();
        public ICollection<DadoCompartilhado> DadosCompartilhados { get; set; } = new List<DadoCompartilhado>();
        public ICollection<HistoricoAcordo> Historicos { get; set; } = new List<HistoricoAcordo>();

        // ============================================================
        // REGRAS DE NEGOCIO (encapsuladas na entidade)
        // ============================================================

        /// <summary>
        /// Verifica se o acordo pode ser ativado (ambas assinaturas presentes).
        /// </summary>
        public bool PodeAtivar()
        {
            var assinaturas = 0;
            foreach (var resp in Responsaveis)
            {
                if (resp.TpPapel == TipoPapel.AssinanteFornecedora && resp.DtAssinatura.HasValue)
                    assinaturas++;
                if (resp.TpPapel == TipoPapel.AssinanteConsumidora && resp.DtAssinatura.HasValue)
                    assinaturas++;
            }
            return assinaturas >= 2 && CoSituacao == SituacaoAcordo.Pendente;
        }

        /// <summary>
        /// Verifica se o acordo pode ser excluido (exclusao logica).
        /// So e permitido se nao estiver assinado por ambas as partes.
        /// </summary>
        public bool PodeExcluir()
        {
            return CoSituacao == SituacaoAcordo.Pendente;
        }

        /// <summary>
        /// Verifica se o acordo pode receber solicitacao de inativacao precoce.
        /// </summary>
        public bool PodeSolicitarInativacao()
        {
            return CoSituacao == SituacaoAcordo.Ativo;
        }
    }
}
