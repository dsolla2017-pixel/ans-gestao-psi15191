// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Historico: Registro de cada mudanca de status do acordo.
// - Trilha de Auditoria: Permite saber quem fez o que e quando.
// ============================================================

using System;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade de auditoria que registra cada mudanca de situacao do ANS.
    /// Mapeada para a tabela TB_ANS_HISTORICO.
    /// </summary>
    public class HistoricoAcordo
    {
        public int CoHistorico { get; set; }
        public int CoAcordo { get; set; }

        // Mudanca de status
        public SituacaoAcordo CoSituacaoAnterior { get; set; }
        public SituacaoAcordo CoSituacaoNova { get; set; }

        // Detalhes
        public string? DeJustificativa { get; set; }
        public DateTime DtAlteracao { get; set; } = DateTime.UtcNow;
        public string NuMatriculaResponsavel { get; set; } = string.Empty;

        // Navegacao
        public Acordo? Acordo { get; set; }
    }
}
