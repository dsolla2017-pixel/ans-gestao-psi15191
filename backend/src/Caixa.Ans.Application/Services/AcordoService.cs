// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Service: Classe que orquestra as regras de negocio.
// - Dependency Injection: O sistema conecta as pecas automaticamente.
// - Async/Await: Operacao que nao bloqueia o servidor enquanto espera.
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caixa.Ans.Application.DTOs;
using Caixa.Ans.Domain.Entities;
using Caixa.Ans.Domain.Enums;
using Caixa.Ans.Domain.Interfaces;

namespace Caixa.Ans.Application.Services
{
    /// <summary>
    /// Servico de aplicacao que implementa os casos de uso do ANS.
    /// Orquestra as chamadas ao repositorio e aplica regras de negocio.
    /// </summary>
    public class AcordoService
    {
        private readonly IAcordoRepository _repository;

        public AcordoService(IAcordoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Cria um novo ANS com situacao Pendente (regra obrigatoria).
        /// </summary>
        public async Task<AcordoResumoDto> CriarAcordoAsync(CriarAcordoDto dto, string matriculaCriador)
        {
            var acordo = new Acordo
            {
                NoFornecedora = dto.NoFornecedora,
                NoConsumidora = dto.NoConsumidora,
                NoUnidadeFornecedora = dto.NoUnidadeFornecedora,
                NoUnidadeConsumidora = dto.NoUnidadeConsumidora,
                QtDiasVigencia = dto.QtDiasVigencia,
                CoGrauSigilo = dto.CoGrauSigilo,
                DePeriodicidade = dto.DePeriodicidade,
                IcDadoPessoal = dto.IcDadoPessoal,
                IcDadoSensivel = dto.IcDadoSensivel,
                CoSituacao = SituacaoAcordo.Pendente,
                NuMatriculaCriador = matriculaCriador,
                DtCriacao = DateTime.UtcNow
            };

            // Adicionar responsaveis tecnicos
            acordo.Responsaveis.Add(new Responsavel
            {
                NoPessoa = dto.ResponsavelFornecedora.NoPessoa,
                NuCpf = dto.ResponsavelFornecedora.NuCpf,
                NuMatricula = dto.ResponsavelFornecedora.NuMatricula,
                NuTelefone = dto.ResponsavelFornecedora.NuTelefone,
                DeEmail = dto.ResponsavelFornecedora.DeEmail,
                TpPapel = TipoPapel.TecnicoFornecedora
            });

            acordo.Responsaveis.Add(new Responsavel
            {
                NoPessoa = dto.ResponsavelConsumidora.NoPessoa,
                NuCpf = dto.ResponsavelConsumidora.NuCpf,
                NuMatricula = dto.ResponsavelConsumidora.NuMatricula,
                NuTelefone = dto.ResponsavelConsumidora.NuTelefone,
                DeEmail = dto.ResponsavelConsumidora.DeEmail,
                TpPapel = TipoPapel.TecnicoConsumidora
            });

            // Adicionar dados compartilhados
            foreach (var dadoDto in dto.DadosCompartilhados)
            {
                acordo.DadosCompartilhados.Add(new DadoCompartilhado
                {
                    NoNegocial = dadoDto.NoNegocial,
                    NoObjetoFisico = dadoDto.NoObjetoFisico,
                    DeLink = dadoDto.DeLink,
                    IcDadoPessoal = dadoDto.IcDadoPessoal,
                    IcDadoSensivel = dadoDto.IcDadoSensivel
                });
            }

            var criado = await _repository.IncluirAsync(acordo);

            return new AcordoResumoDto
            {
                CoAcordo = criado.CoAcordo,
                NoFornecedora = criado.NoFornecedora,
                NoConsumidora = criado.NoConsumidora,
                Situacao = criado.CoSituacao.ToString(),
                DtCriacao = criado.DtCriacao,
                QtDiasVigencia = criado.QtDiasVigencia,
                IcDadoPessoal = criado.IcDadoPessoal,
                IcDadoSensivel = criado.IcDadoSensivel
            };
        }

        /// <summary>
        /// Lista todos os ANS com filtros opcionais.
        /// </summary>
        public async Task<IEnumerable<AcordoResumoDto>> ListarAcordosAsync(
            string? filtro, string? situacao, string? ordenarPor)
        {
            var acordos = await _repository.ListarTodosAsync(filtro, situacao, ordenarPor);

            return acordos.Select(a => new AcordoResumoDto
            {
                CoAcordo = a.CoAcordo,
                NoFornecedora = a.NoFornecedora,
                NoConsumidora = a.NoConsumidora,
                Situacao = a.CoSituacao.ToString(),
                DtCriacao = a.DtCriacao,
                DtAssinatura = a.DtAssinatura,
                DtFimVigencia = a.DtFimVigencia,
                QtDiasVigencia = a.QtDiasVigencia,
                IcDadoPessoal = a.IcDadoPessoal,
                IcDadoSensivel = a.IcDadoSensivel
            });
        }

        /// <summary>
        /// Obtem os detalhes completos de um ANS.
        /// </summary>
        public async Task<AcordoDetalheDto?> ObterDetalheAsync(int coAcordo)
        {
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null) return null;

            return new AcordoDetalheDto
            {
                CoAcordo = acordo.CoAcordo,
                NoFornecedora = acordo.NoFornecedora,
                NoConsumidora = acordo.NoConsumidora,
                NoUnidadeFornecedora = acordo.NoUnidadeFornecedora,
                NoUnidadeConsumidora = acordo.NoUnidadeConsumidora,
                Situacao = acordo.CoSituacao.ToString(),
                DtCriacao = acordo.DtCriacao,
                DtAssinatura = acordo.DtAssinatura,
                DtFimVigencia = acordo.DtFimVigencia,
                QtDiasVigencia = acordo.QtDiasVigencia,
                CoGrauSigilo = acordo.CoGrauSigilo,
                DePeriodicidade = acordo.DePeriodicidade,
                IcDadoPessoal = acordo.IcDadoPessoal,
                IcDadoSensivel = acordo.IcDadoSensivel,
                DeJustificativaInativacao = acordo.DeJustificativaInativacao,
                Responsaveis = acordo.Responsaveis.Select(r => new ResponsavelDto
                {
                    NoPessoa = r.NoPessoa,
                    NuCpf = r.NuCpf,
                    NuMatricula = r.NuMatricula,
                    NuTelefone = r.NuTelefone,
                    DeEmail = r.DeEmail,
                    NoCargoFuncao = r.NoCargoFuncao,
                    TpPapel = r.TpPapel.ToString(),
                    DtAssinatura = r.DtAssinatura
                }).ToList(),
                DadosCompartilhados = acordo.DadosCompartilhados.Select(d => new DadoCompartilhadoDto
                {
                    NoNegocial = d.NoNegocial,
                    NoObjetoFisico = d.NoObjetoFisico,
                    DeLink = d.DeLink,
                    IcDadoPessoal = d.IcDadoPessoal,
                    IcDadoSensivel = d.IcDadoSensivel
                }).ToList()
            };
        }

        /// <summary>
        /// Registra a assinatura de uma das partes.
        /// Se ambas as partes assinaram, ativa o ANS automaticamente.
        /// </summary>
        public async Task<bool> AssinarAcordoAsync(int coAcordo, string matricula, string papel)
        {
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null) return false;

            var tipoPapel = papel == "Fornecedora"
                ? TipoPapel.AssinanteFornecedora
                : TipoPapel.AssinanteConsumidora;

            // Verificar se ja existe assinante para este papel
            var assinante = acordo.Responsaveis
                .FirstOrDefault(r => r.TpPapel == tipoPapel);

            if (assinante != null)
            {
                assinante.DtAssinatura = DateTime.UtcNow;
                assinante.NuMatricula = matricula;
            }

            // Verificar se ambas as partes assinaram para ativar
            if (acordo.PodeAtivar())
            {
                acordo.CoSituacao = SituacaoAcordo.Ativo;
                acordo.DtAssinatura = DateTime.UtcNow;
                acordo.DtFimVigencia = DateTime.UtcNow.AddDays(acordo.QtDiasVigencia);
            }

            await _repository.AtualizarAsync(acordo);
            return true;
        }

        /// <summary>
        /// Solicita a inativacao precoce de um ANS (dupla-custodia).
        /// </summary>
        public async Task<bool> SolicitarInativacaoAsync(
            int coAcordo, SolicitarInativacaoDto dto, string matriculaSolicitante)
        {
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null || !acordo.PodeSolicitarInativacao()) return false;

            acordo.CoSituacao = SituacaoAcordo.PendenteInativacao;
            acordo.DeJustificativaInativacao = dto.DeJustificativa;
            acordo.NuMatriculaGerenteAprovador = dto.NuMatriculaGerenteAprovador;

            // Registrar no historico
            acordo.Historicos.Add(new HistoricoAcordo
            {
                CoSituacaoAnterior = SituacaoAcordo.Ativo,
                CoSituacaoNova = SituacaoAcordo.PendenteInativacao,
                DeJustificativa = dto.DeJustificativa,
                NuMatriculaResponsavel = matriculaSolicitante,
                DtAlteracao = DateTime.UtcNow
            });

            await _repository.AtualizarAsync(acordo);
            return true;
        }

        /// <summary>
        /// Gerente avalia (aprova ou recusa) a inativacao precoce.
        /// </summary>
        public async Task<bool> AvaliarInativacaoAsync(
            int coAcordo, AvaliarInativacaoDto dto, string matriculaGerente)
        {
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null || acordo.CoSituacao != SituacaoAcordo.PendenteInativacao)
                return false;

            var novaSituacao = dto.Aprovado
                ? SituacaoAcordo.Inativo
                : SituacaoAcordo.Ativo;

            acordo.Historicos.Add(new HistoricoAcordo
            {
                CoSituacaoAnterior = SituacaoAcordo.PendenteInativacao,
                CoSituacaoNova = novaSituacao,
                DeJustificativa = dto.Aprovado ? "Inativacao aprovada" : "Inativacao recusada",
                NuMatriculaResponsavel = matriculaGerente,
                DtAlteracao = DateTime.UtcNow
            });

            acordo.CoSituacao = novaSituacao;
            await _repository.AtualizarAsync(acordo);
            return true;
        }

        /// <summary>
        /// Exclui logicamente um ANS (somente se estiver pendente).
        /// </summary>
        public async Task<bool> ExcluirAcordoAsync(int coAcordo, string matricula)
        {
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null || !acordo.PodeExcluir()) return false;

            acordo.Historicos.Add(new HistoricoAcordo
            {
                CoSituacaoAnterior = acordo.CoSituacao,
                CoSituacaoNova = SituacaoAcordo.Excluido,
                DeJustificativa = "Exclusao logica solicitada",
                NuMatriculaResponsavel = matricula,
                DtAlteracao = DateTime.UtcNow
            });

            await _repository.ExcluirLogicoAsync(coAcordo);
            return true;
        }
    }
}
