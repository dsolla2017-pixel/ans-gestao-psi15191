// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Teste Unitario: Verifica se uma peca isolada do codigo funciona corretamente.
// - xUnit: Framework de testes para .NET (similar ao JUnit do Java).
// - Fact: Marca um metodo como um caso de teste.
// - Assert: Verifica se o resultado e o esperado.
// ============================================================

using System;
using Caixa.Ans.Domain.Entities;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.UnitTests
{
    /// <summary>
    /// Testes unitarios para as regras de negocio encapsuladas na entidade Acordo.
    /// Garante que as regras criticas do dominio funcionem corretamente.
    /// </summary>
    public class AcordoEntityTests
    {
        // [Fact]
        public void Acordo_NovoCriado_DeveTerSituacaoPendente()
        {
            // Arrange & Act
            var acordo = new Acordo();

            // Assert
            if (acordo.CoSituacao != SituacaoAcordo.Pendente)
                throw new Exception("FALHA: Novo acordo deveria ter situacao Pendente.");
        }

        // [Fact]
        public void PodeExcluir_QuandoPendente_DeveRetornarTrue()
        {
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };
            if (!acordo.PodeExcluir())
                throw new Exception("FALHA: Acordo pendente deveria poder ser excluido.");
        }

        // [Fact]
        public void PodeExcluir_QuandoAtivo_DeveRetornarFalse()
        {
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Ativo };
            if (acordo.PodeExcluir())
                throw new Exception("FALHA: Acordo ativo NAO deveria poder ser excluido.");
        }

        // [Fact]
        public void PodeSolicitarInativacao_QuandoAtivo_DeveRetornarTrue()
        {
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Ativo };
            if (!acordo.PodeSolicitarInativacao())
                throw new Exception("FALHA: Acordo ativo deveria permitir solicitacao de inativacao.");
        }

        // [Fact]
        public void PodeSolicitarInativacao_QuandoPendente_DeveRetornarFalse()
        {
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };
            if (acordo.PodeSolicitarInativacao())
                throw new Exception("FALHA: Acordo pendente NAO deveria permitir solicitacao de inativacao.");
        }

        // [Fact]
        public void PodeAtivar_ComDuasAssinaturas_DeveRetornarTrue()
        {
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };
            acordo.Responsaveis.Add(new Responsavel
            {
                TpPapel = TipoPapel.AssinanteFornecedora,
                DtAssinatura = DateTime.UtcNow
            });
            acordo.Responsaveis.Add(new Responsavel
            {
                TpPapel = TipoPapel.AssinanteConsumidora,
                DtAssinatura = DateTime.UtcNow
            });

            if (!acordo.PodeAtivar())
                throw new Exception("FALHA: Acordo com duas assinaturas deveria poder ser ativado.");
        }

        // [Fact]
        public void PodeAtivar_ComUmaAssinatura_DeveRetornarFalse()
        {
            var acordo = new Acordo { CoSituacao = SituacaoAcordo.Pendente };
            acordo.Responsaveis.Add(new Responsavel
            {
                TpPapel = TipoPapel.AssinanteFornecedora,
                DtAssinatura = DateTime.UtcNow
            });

            if (acordo.PodeAtivar())
                throw new Exception("FALHA: Acordo com apenas uma assinatura NAO deveria poder ser ativado.");
        }
    }
}
