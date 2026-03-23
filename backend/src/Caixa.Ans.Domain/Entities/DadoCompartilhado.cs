// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - DadoCompartilhado: Informacao que sera trocada entre as partes do acordo.
// - Objeto Fisico: Nome da tabela, arquivo ou recurso que contem o dado.
// - Link: Endereco web para acessar o dado, quando disponivel.
// ============================================================

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade que detalha os dados objeto do compartilhamento no ANS.
    /// Mapeada para a tabela TB_ANS_DADO_COMPARTILHADO.
    /// </summary>
    public class DadoCompartilhado
    {
        public int CoDado { get; set; }
        public int CoAcordo { get; set; }

        // Identificacao do dado
        public string NoNegocial { get; set; } = string.Empty;
        public string NoObjetoFisico { get; set; } = string.Empty;
        public string? DeLink { get; set; }

        // Classificacao
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }

        // Navegacao
        public Acordo? Acordo { get; set; }
    }
}
