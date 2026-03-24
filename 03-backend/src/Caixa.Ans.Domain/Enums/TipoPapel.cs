// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [RBAC] Role-Based Access Control:
//   Cada papel define as permissões e responsabilidades do
//   participante no contexto do ANS. A distinção entre Técnico
//   e Assinante implementa a Segregação de Funções (SoD).
// ============================================================

namespace Caixa.Ans.Domain.Enums
{
    /// <summary>
    /// Enum que define os papéis que um responsável pode exercer
    /// no Acordo de Nível de Serviço.
    /// 
    /// A separação entre papéis Técnicos e Assinantes implementa
    /// o princípio de Segregação de Funções (Separation of Duties),
    /// garantindo que operações críticas como a assinatura do ANS
    /// sejam realizadas exclusivamente por chefes de unidade.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public enum TipoPapel
    {
        /// <summary>Responsável técnico pela parte Fornecedora dos dados — ponto focal operacional.</summary>
        TecnicoFornecedora = 1,

        /// <summary>Responsável técnico pela parte Consumidora dos dados — ponto focal operacional.</summary>
        TecnicoConsumidora = 2,

        /// <summary>Chefe de unidade da Fornecedora — autoridade para assinar o ANS.</summary>
        AssinanteFornecedora = 3,

        /// <summary>Chefe de unidade da Consumidora — autoridade para assinar o ANS.</summary>
        AssinanteConsumidora = 4
    }
}
