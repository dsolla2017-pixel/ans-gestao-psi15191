// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Event Sourcing Lite] Cada transição de estado do Acordo gera
//   um registro imutável nesta tabela, criando uma trilha de
//   auditoria completa. Embora não seja Event Sourcing puro,
//   adota o princípio de append-only para rastreabilidade.
//
// [Audit Trail] OWASP Logging Cheat Sheet:
//   Registra quem (NuMatriculaResponsavel), o quê (transição de
//   estado), quando (DtAlteracao) e por quê (DeJustificativa).
//
// [Nomenclatura CAIXA] Guia de Nomenclatura de Objetos SGBD:
//   Prefixos: CO_ (código), DE_ (descrição), DT_ (data), NU_ (número).
// ============================================================

using System;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade de auditoria que registra cada transição de estado
    /// do Acordo de Nível de Serviço.
    /// 
    /// Implementa o padrão Audit Trail (Trilha de Auditoria) para
    /// garantir rastreabilidade completa do ciclo de vida do ANS.
    /// Cada registro é imutável após a criação (append-only),
    /// preservando a integridade do histórico.
    /// 
    /// Mapeada para a tabela TB_ANS_HISTORICO.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class HistoricoAcordo
    {
        // ── Identificação ──────────────────────────────────────────
        // Chave primária surrogate gerada pelo banco de dados.
        public int CoHistorico { get; set; }

        // Chave estrangeira para o Acordo que sofreu a transição.
        public int CoAcordo { get; set; }

        // ── Transição de Estado ────────────────────────────────────
        // Registra o estado anterior e o novo estado do acordo.
        // Essa dupla permite reconstruir a linha do tempo completa
        // de cada ANS, desde a criação até a eventual inativação.
        // Exemplo: Pendente → Ativo, Ativo → PendenteInativacao.
        public SituacaoAcordo CoSituacaoAnterior { get; set; }
        public SituacaoAcordo CoSituacaoNova { get; set; }

        // ── Contexto da Alteração ──────────────────────────────────
        // Justificativa textual para a transição de estado.
        // Obrigatória para inativação precoce (requisito de dupla-custódia).
        // Nullable porque transições como Pendente → Ativo são automáticas.
        public string? DeJustificativa { get; set; }

        // Registro temporal da alteração em UTC.
        // Utiliza UTC para consistência em ambientes distribuídos
        // e para evitar ambiguidades com horário de verão.
        public DateTime DtAlteracao { get; set; } = DateTime.UtcNow;

        // Matrícula do empregado responsável pela alteração.
        // Garante accountability individual conforme as diretrizes
        // de governança corporativa da CAIXA e princípios da LGPD.
        public string NuMatriculaResponsavel { get; set; } = string.Empty;

        // ── Navegação (Entity Framework Core) ──────────────────────
        // Navigation property para o Acordo que originou este registro.
        public Acordo? Acordo { get; set; }
    }
}
