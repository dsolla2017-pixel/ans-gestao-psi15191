// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================

namespace Caixa.Ans.Domain.Enums
{
    /// <summary>
    /// Situacoes possiveis de um Acordo de Nivel de Servico.
    /// </summary>
    public enum SituacaoAcordo
    {
        Pendente = 1,
        Ativo = 2,
        Inativo = 3,
        Excluido = 4,
        PendenteInativacao = 5
    }
}
