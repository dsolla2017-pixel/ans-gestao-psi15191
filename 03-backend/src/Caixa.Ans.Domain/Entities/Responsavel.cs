// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Responsavel: Pessoa que participa do acordo (tecnico, assinante).
// - FK (Foreign Key): Referencia que conecta esta tabela ao Acordo.
// - Enum TipoPapel: Define se a pessoa e tecnico ou assinante.
// ============================================================

using System;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um responsavel vinculado ao ANS.
    /// Pode ser responsavel tecnico ou assinante (chefe de unidade).
    /// Mapeada para a tabela TB_ANS_RESPONSAVEL.
    /// </summary>
    public class Responsavel
    {
        public int CoResponsavel { get; set; }
        public int CoAcordo { get; set; }

        // Dados pessoais do responsavel
        public string NoPessoa { get; set; } = string.Empty;
        public string NuCpf { get; set; } = string.Empty;
        public string? NuMatricula { get; set; }
        public string? NuTelefone { get; set; }
        public string? DeEmail { get; set; }
        public string? NoCargoFuncao { get; set; }

        // Papel no acordo
        public TipoPapel TpPapel { get; set; }

        // Assinatura (preenchida quando o chefe assina)
        public DateTime? DtAssinatura { get; set; }

        // Navegacao
        public Acordo? Acordo { get; set; }
    }
}
