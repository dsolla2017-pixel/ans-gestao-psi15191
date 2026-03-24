// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [State Machine] Cada valor deste enum representa um estado
//   válido na máquina de estados do Acordo. As transições
//   permitidas são controladas pela entidade Acordo (Aggregate Root).
//
// [Explicit Numbering] Os valores numéricos são atribuídos
//   explicitamente para garantir estabilidade na persistência.
//   Novos estados devem ser adicionados ao final da sequência,
//   nunca reutilizando valores existentes (cf. Microsoft C# Guidelines).
// ============================================================

namespace Caixa.Ans.Domain.Enums
{
    /// <summary>
    /// Enum que define os estados possíveis do ciclo de vida
    /// de um Acordo de Nível de Serviço (ANS).
    /// 
    /// Máquina de Estados:
    /// ┌──────────┐    Duas assinaturas    ┌────────┐
    /// │ Pendente │ ──────────────────────► │ Ativo  │
    /// └──────────┘                         └────────┘
    ///      │                                    │
    ///      │ Exclusão lógica                    │ Solicitação de inativação
    ///      ▼                                    ▼
    /// ┌──────────┐                    ┌─────────────────────┐
    /// │ Excluído │                    │ PendenteInativação  │
    /// └──────────┘                    └─────────────────────┘
    ///                                           │
    ///                                           │ Autorização gerencial
    ///                                           ▼
    ///                                      ┌──────────┐
    ///                                      │ Inativo  │
    ///                                      └──────────┘
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public enum SituacaoAcordo
    {
        /// <summary>Estado inicial: ANS criado, aguarda assinaturas bilaterais.</summary>
        Pendente = 1,

        /// <summary>ANS ativo: ambas as partes assinaram o acordo.</summary>
        Ativo = 2,

        /// <summary>ANS inativado: encerrado por término de vigência ou inativação precoce autorizada.</summary>
        Inativo = 3,

        /// <summary>ANS excluído logicamente: removido antes da assinatura bilateral (soft delete).</summary>
        Excluido = 4,

        /// <summary>ANS com solicitação de inativação precoce: aguarda autorização gerencial (dupla-custódia).</summary>
        PendenteInativacao = 5
    }
}
