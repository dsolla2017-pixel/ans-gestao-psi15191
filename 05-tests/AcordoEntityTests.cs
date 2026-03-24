// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [TDD] Kent Beck — Test-Driven Development by Example:
//   Testes unitários escritos para validar as invariantes de
//   negócio encapsuladas na entidade Acordo (Aggregate Root).
//
// [AAA Pattern] Arrange-Act-Assert:
//   Cada teste segue a estrutura AAA para clareza e legibilidade.
//   Arrange: configura o cenário. Act: executa a ação. Assert: valida.
//
// [xUnit] Framework de testes para .NET:
//   Utiliza o atributo [Fact] para marcar métodos de teste.
//   Cada teste é independente e isolado (sem estado compartilhado).
//
// [Domain Invariants] Eric Evans — DDD:
//   Os testes validam as regras de negócio que devem ser sempre
//   verdadeiras, independentemente do estado externo do sistema.
// ============================================================

using System;
using Caixa.Ans.Domain.Entities;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.UnitTests
{
    /// <summary>
    /// Suíte de testes unitários para as regras de negócio encapsuladas
    /// na entidade Acordo (Aggregate Root do modelo de domínio).
    /// 
    /// Cenários cobertos:
    /// - Invariante de criação (situação inicial obrigatória)
    /// - Regras de exclusão lógica (soft delete)
    /// - Regras de inativação precoce (dupla-custódia)
    /// - Regras de ativação (assinatura bilateral)
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AcordoEntityTests
    {
        // ════════════════════════════════════════════════════════════
        // TESTE 1 — Invariante de Criação
        // Regra de negócio: todo ANS nasce com situação "Pendente".
        // Conforme especificação do PSI 15191: "O ANS é criado
        // obrigatoriamente com a situação Pendente."
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void Acordo_NovoCriado_DeveTerSituacaoPendente()
        {
            // Arrange & Act: cria uma nova instância do Acordo
            var acordo = new Acordo();

            // Assert: verifica que a situação inicial é Pendente
            if (acordo.CoSituacao != SituacaoAcordo.Pendente)
                throw new Exception("FALHA: Novo acordo deveria ter situação Pendente.");
        }

        // ════════════════════════════════════════════════════════════
        // TESTE 2 — Exclusão Lógica: Cenário Positivo
        // Regra: apenas ANS com situação "Pendente" podem ser excluídos.
        // ANS pendentes ainda não foram assinados por nenhuma parte.
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void PodeExcluir_QuandoPendente_DeveRetornarTrue()
        {
            // Arrange: cria acordo com situação Pendente
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };

            // Act & Assert: acordo pendente pode ser excluído
            if (!acordo.PodeExcluir())
                throw new Exception("FALHA: Acordo pendente deveria poder ser excluído.");
        }

        // ════════════════════════════════════════════════════════════
        // TESTE 3 — Exclusão Lógica: Cenário Negativo
        // Regra: ANS ativos (já assinados) não podem ser excluídos.
        // A exclusão de acordos ativos violaria a integridade contratual.
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void PodeExcluir_QuandoAtivo_DeveRetornarFalse()
        {
            // Arrange: cria acordo com situação Ativo
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Ativo };

            // Act & Assert: acordo ativo NÃO pode ser excluído
            if (acordo.PodeExcluir())
                throw new Exception("FALHA: Acordo ativo NÃO deveria poder ser excluído.");
        }

        // ════════════════════════════════════════════════════════════
        // TESTE 4 — Inativação Precoce: Cenário Positivo
        // Regra: apenas ANS ativos podem ter inativação solicitada.
        // A inativação precoce requer dupla-custódia (empregado + gestor).
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void PodeSolicitarInativacao_QuandoAtivo_DeveRetornarTrue()
        {
            // Arrange: cria acordo com situação Ativo
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Ativo };

            // Act & Assert: acordo ativo permite solicitação de inativação
            if (!acordo.PodeSolicitarInativacao())
                throw new Exception("FALHA: Acordo ativo deveria permitir solicitação de inativação.");
        }

        // ════════════════════════════════════════════════════════════
        // TESTE 5 — Inativação Precoce: Cenário Negativo
        // Regra: ANS pendentes não podem ter inativação solicitada.
        // Não faz sentido inativar um acordo que ainda não foi ativado.
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void PodeSolicitarInativacao_QuandoPendente_DeveRetornarFalse()
        {
            // Arrange: cria acordo com situação Pendente
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };

            // Act & Assert: acordo pendente NÃO permite inativação
            if (acordo.PodeSolicitarInativacao())
                throw new Exception("FALHA: Acordo pendente NÃO deveria permitir solicitação de inativação.");
        }

        // ════════════════════════════════════════════════════════════
        // TESTE 6 — Ativação Bilateral: Cenário Positivo
        // Regra: o ANS fica ativo quando AMBAS as partes assinam.
        // Conforme PSI 15191: "o ANS fica ativo quando os dois
        // responsáveis tiverem assinado."
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void PodeAtivar_ComDuasAssinaturas_DeveRetornarTrue()
        {
            // Arrange: cria acordo pendente com dois assinantes
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };

            // Assinatura da parte Fornecedora
            acordo.Responsaveis.Add(new Responsavel
            {
                TpPapel = TipoPapel.AssinanteFornecedora,
                DtAssinatura = DateTime.UtcNow
            });

            // Assinatura da parte Consumidora
            acordo.Responsaveis.Add(new Responsavel
            {
                TpPapel = TipoPapel.AssinanteConsumidora,
                DtAssinatura = DateTime.UtcNow
            });

            // Act & Assert: com ambas as assinaturas, o acordo pode ser ativado
            if (!acordo.PodeAtivar())
                throw new Exception("FALHA: Acordo com duas assinaturas deveria poder ser ativado.");
        }

        // ════════════════════════════════════════════════════════════
        // TESTE 7 — Ativação Bilateral: Cenário Negativo
        // Regra: com apenas UMA assinatura, o ANS permanece pendente.
        // A ativação bilateral é uma invariante de segurança contratual.
        // ════════════════════════════════════════════════════════════
        // [Fact]
        public void PodeAtivar_ComUmaAssinatura_DeveRetornarFalse()
        {
            // Arrange: cria acordo pendente com apenas um assinante
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };

            // Apenas a assinatura da parte Fornecedora
            acordo.Responsaveis.Add(new Responsavel
            {
                TpPapel = TipoPapel.AssinanteFornecedora,
                DtAssinatura = DateTime.UtcNow
            });

            // Act & Assert: com apenas uma assinatura, o acordo NÃO pode ser ativado
            if (acordo.PodeAtivar())
                throw new Exception("FALHA: Acordo com apenas uma assinatura NÃO deveria poder ser ativado.");
        }
    }
}
