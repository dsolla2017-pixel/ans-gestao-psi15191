// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [DDD] Value Object candidato dentro do Aggregate de Acordo:
//   Cada DadoCompartilhado descreve um ativo de informação que
//   transita entre as partes do ANS. Seu ciclo de vida é controlado
//   pelo Aggregate Root (Acordo).
//
// [LGPD] Lei 13.709/2018 — Classificação de Dados:
//   Os indicadores IcDadoPessoal e IcDadoSensivel permitem
//   classificação nativa no momento do cadastro, atendendo ao
//   princípio de Privacy by Design (Ann Cavoukian, 2009).
//
// [Nomenclatura CAIXA] Guia de Nomenclatura de Objetos SGBD:
//   Prefixos: CO_ (código), NO_ (nome), DE_ (descrição), IC_ (indicador).
// ============================================================

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade que descreve cada ativo de informação objeto do
    /// compartilhamento no Acordo de Nível de Serviço.
    /// 
    /// Conforme o PSI 15191, cada dado compartilhado deve conter:
    /// - Nome negocial do dado
    /// - Nome do objeto físico (arquivo, tabela, recurso)
    /// - Link para o objeto (quando disponível)
    /// - Classificação LGPD (dado pessoal e/ou sensível)
    /// 
    /// Mapeada para a tabela TB_ANS_DADO_COMPARTILHADO.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class DadoCompartilhado
    {
        // ── Identificação ──────────────────────────────────────────
        // Chave primária surrogate gerada pelo banco de dados.
        public int CoDado { get; set; }

        // Chave estrangeira para o Acordo (Aggregate Root).
        // Garante que todo dado compartilhado pertença a um ANS válido.
        public int CoAcordo { get; set; }

        // ── Descrição do Ativo de Informação ───────────────────────
        // Nome negocial: identificação do dado na linguagem do negócio.
        // Exemplo: "Base de Clientes PF", "Carteira de Crédito Imobiliário".
        public string NoNegocial { get; set; } = string.Empty;

        // Nome do objeto físico que contém o dado no ambiente tecnológico.
        // Pode ser uma tabela de banco de dados, arquivo CSV, API, etc.
        // Exemplo: "TB_CLIENTE_PF", "carteira_credito_202503.csv".
        public string NoObjetoFisico { get; set; } = string.Empty;

        // Link para acesso ao objeto físico, quando disponível.
        // Nullable porque nem todos os ativos possuem URL de acesso direto.
        // Quando preenchido, facilita a rastreabilidade e auditoria do dado.
        public string? DeLink { get; set; }

        // ── Classificação LGPD ─────────────────────────────────────
        // Indicadores de conformidade com a Lei Geral de Proteção de Dados.
        // Implementam o conceito de Privacy by Design: a classificação
        // ocorre no momento do cadastro, não como etapa posterior.
        // Essa abordagem reduz o risco de tratamento inadequado de dados.

        // Indica se o dado contém informações pessoais (Art. 5º, I, LGPD).
        // Exemplo: nome, CPF, endereço, telefone de pessoa natural.
        public bool IcDadoPessoal { get; set; }

        // Indica se o dado contém informações pessoais sensíveis (Art. 5º, II, LGPD).
        // Exemplo: dados de saúde, biometria, convicção religiosa, filiação sindical.
        public bool IcDadoSensivel { get; set; }

        // ── Navegação (Entity Framework Core) ──────────────────────
        // Navigation property para o Aggregate Root.
        // Permite consultas LINQ bidirecionais e carregamento otimizado.
        public Acordo? Acordo { get; set; }
    }
}
