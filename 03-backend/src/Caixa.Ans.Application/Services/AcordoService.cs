// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Application Service] DDD — Eric Evans, Cap. 7:
//   Serviço de aplicação que orquestra casos de uso sem conter
//   regras de negócio. As invariantes residem nas entidades;
//   este serviço coordena a interação entre DTOs e repositório.
//
// [Single Responsibility Principle] SOLID — Robert C. Martin:
//   Cada método corresponde a um caso de uso específico do PSI 15191.
//
// [Constructor Injection] Dependency Injection Pattern:
//   O repositório é injetado via construtor, permitindo substituição
//   por mocks em testes unitários (Testability by Design).
//
// [Async/Await] TAP — Task-based Asynchronous Pattern:
//   Todas as operações são assíncronas para escalabilidade do servidor.
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
    /// Serviço de aplicação que implementa os casos de uso do
    /// Acordo de Nível de Serviço, conforme especificação do PSI 15191.
    /// 
    /// Casos de uso implementados:
    /// 1. Incluir ANS (com situação Pendente obrigatória)
    /// 2. Listar ANS (com filtros, ordenação e busca)
    /// 3. Consultar detalhes do ANS
    /// 4. Assinar ANS (com ativação automática por dupla assinatura)
    /// 5. Solicitar inativação precoce (dupla-custódia)
    /// 6. Avaliar inativação (autorizar ou recusar)
    /// 7. Excluir ANS (exclusão lógica)
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AcordoService
    {
        // Dependência injetada via construtor (Dependency Inversion Principle).
        // A interface IAcordoRepository permite substituição por mocks em testes.
        private readonly IAcordoRepository _repository;

        /// <summary>
        /// Construtor com injeção de dependência do repositório.
        /// O container de DI do ASP.NET Core resolve automaticamente
        /// a implementação concreta registrada em Program.cs.
        /// </summary>
        /// <param name="repository">Implementação do repositório de Acordos.</param>
        public AcordoService(IAcordoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Caso de uso: Incluir novo ANS.
        /// 
        /// Regra de negócio: O ANS é criado obrigatoriamente com
        /// situação "Pendente", conforme especificação do PSI 15191.
        /// Os responsáveis técnicos são vinculados no momento da criação.
        /// 
        /// Fluxo:
        /// 1. Mapeia o DTO de entrada para a entidade de domínio
        /// 2. Define a situação inicial como Pendente (invariante)
        /// 3. Vincula os responsáveis técnicos de ambas as partes
        /// 4. Vincula os dados objeto do compartilhamento
        /// 5. Persiste via repositório e retorna o DTO de resumo
        /// </summary>
        /// <param name="dto">Dados do formulário de inclusão.</param>
        /// <param name="matriculaCriador">Matrícula do empregado GEGOD que cria o ANS.</param>
        /// <returns>DTO resumido do ANS criado, com identificador gerado.</returns>
        public async Task<AcordoResumoDto> CriarAcordoAsync(CriarAcordoDto dto, string matriculaCriador)
        {
            // Mapeamento DTO → Entidade de Domínio (Anti-Corruption Layer)
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
                CoSituacao = SituacaoAcordo.Pendente,   // Invariante: sempre Pendente na criação
                NuMatriculaCriador = matriculaCriador,   // Rastreabilidade do criador
                DtCriacao = DateTime.UtcNow              // UTC para consistência temporal
            };

            // Vincula o responsável técnico da parte Fornecedora dos dados
            acordo.Responsaveis.Add(new Responsavel
            {
                NoPessoa = dto.ResponsavelFornecedora.NoPessoa,
                NuCpf = dto.ResponsavelFornecedora.NuCpf,
                NuMatricula = dto.ResponsavelFornecedora.NuMatricula,
                NuTelefone = dto.ResponsavelFornecedora.NuTelefone,
                DeEmail = dto.ResponsavelFornecedora.DeEmail,
                TpPapel = TipoPapel.TecnicoFornecedora
            });

            // Vincula o responsável técnico da parte Consumidora dos dados
            acordo.Responsaveis.Add(new Responsavel
            {
                NoPessoa = dto.ResponsavelConsumidora.NoPessoa,
                NuCpf = dto.ResponsavelConsumidora.NuCpf,
                NuMatricula = dto.ResponsavelConsumidora.NuMatricula,
                NuTelefone = dto.ResponsavelConsumidora.NuTelefone,
                DeEmail = dto.ResponsavelConsumidora.DeEmail,
                TpPapel = TipoPapel.TecnicoConsumidora
            });

            // Vincula cada ativo de informação objeto do compartilhamento
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

            // Persiste o agregado completo no banco de dados
            var criado = await _repository.IncluirAsync(acordo);

            // Retorna o DTO resumido com o identificador gerado pelo banco
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
        /// Caso de uso: Listar todos os ANS com filtros opcionais.
        /// 
        /// Atende ao requisito do PSI 15191: "permitir ao empregado
        /// consultar todas as ANS firmadas entre a CAIXA e as coparticipadas,
        /// com ordenação por coparticipada ou data de vigência e filtro
        /// por palavras-chave em qualquer campo do resultado".
        /// </summary>
        /// <param name="filtro">Texto livre para busca em qualquer campo.</param>
        /// <param name="situacao">Filtro por situação do ANS.</param>
        /// <param name="ordenarPor">Campo de ordenação.</param>
        /// <returns>Coleção de DTOs resumidos para exibição na listagem.</returns>
        public async Task<IEnumerable<AcordoResumoDto>> ListarAcordosAsync(
            string? filtro, string? situacao, string? ordenarPor)
        {
            // Delega a consulta ao repositório (separação de responsabilidades)
            var acordos = await _repository.ListarTodosAsync(filtro, situacao, ordenarPor);

            // Projeção: Entidade → DTO (protege o domínio de exposição externa)
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
        /// Caso de uso: Consultar detalhes completos de um ANS.
        /// 
        /// Retorna todas as informações do acordo, incluindo responsáveis,
        /// dados compartilhados, classificação LGPD e situação atual.
        /// Atende ao requisito "Consultar detalhes" do PSI 15191.
        /// </summary>
        /// <param name="coAcordo">Identificador único do Acordo.</param>
        /// <returns>DTO detalhado ou null se não encontrado.</returns>
        public async Task<AcordoDetalheDto?> ObterDetalheAsync(int coAcordo)
        {
            // Recupera o agregado completo com eager loading
            var acordo = await _repository.ObterPorIdAsync(coAcordo);

            // Guard Clause: retorna null para acordos inexistentes
            if (acordo == null) return null;

            // Mapeamento completo: Entidade → DTO de detalhamento
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

                // Projeção dos responsáveis vinculados ao acordo
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

                // Projeção dos dados compartilhados
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
        /// Caso de uso: Registrar assinatura de uma das partes.
        /// 
        /// Quando ambas as partes (Fornecedora e Consumidora) assinam,
        /// o ANS é ativado automaticamente. Essa regra implementa o
        /// requisito de consenso bilateral do PSI 15191.
        /// 
        /// A verificação de pré-condição (PodeAtivar) é delegada à
        /// entidade de domínio, respeitando o Rich Domain Model.
        /// </summary>
        /// <param name="coAcordo">Identificador do Acordo.</param>
        /// <param name="matricula">Matrícula do chefe de unidade que assina.</param>
        /// <param name="papel">Papel: "Fornecedora" ou "Consumidora".</param>
        /// <returns>True se a assinatura foi registrada com sucesso.</returns>
        public async Task<bool> AssinarAcordoAsync(int coAcordo, string matricula, string papel)
        {
            // Recupera o agregado para aplicar a regra de negócio
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null) return false;

            // Determina o tipo de papel com base no parâmetro recebido
            var tipoPapel = papel == "Fornecedora"
                ? TipoPapel.AssinanteFornecedora
                : TipoPapel.AssinanteConsumidora;

            // Localiza o assinante correspondente ao papel informado
            var assinante = acordo.Responsaveis
                .FirstOrDefault(r => r.TpPapel == tipoPapel);

            // Registra a assinatura com timestamp UTC
            if (assinante != null)
            {
                assinante.DtAssinatura = DateTime.UtcNow;
                assinante.NuMatricula = matricula;
            }

            // Verifica se ambas as assinaturas foram registradas (invariante do domínio)
            if (acordo.PodeAtivar())
            {
                acordo.CoSituacao = SituacaoAcordo.Ativo;
                acordo.DtAssinatura = DateTime.UtcNow;
                acordo.DtFimVigencia = DateTime.UtcNow.AddDays(acordo.QtDiasVigencia);
            }

            // Persiste as alterações no banco de dados
            await _repository.AtualizarAsync(acordo);
            return true;
        }

        /// <summary>
        /// Caso de uso: Solicitar inativação precoce (dupla-custódia).
        /// 
        /// Primeiro passo do fluxo de Segregação de Funções:
        /// o empregado GEGOD fornece a justificativa e indica o gestor
        /// com função gerencial que deve autorizar a inativação.
        /// 
        /// O ANS transita de "Ativo" para "PendenteInativacao".
        /// A transição é registrada no histórico de auditoria.
        /// </summary>
        /// <param name="coAcordo">Identificador do Acordo.</param>
        /// <param name="dto">Justificativa e matrícula do gestor aprovador.</param>
        /// <param name="matriculaSolicitante">Matrícula do empregado solicitante.</param>
        /// <returns>True se a solicitação foi registrada com sucesso.</returns>
        public async Task<bool> SolicitarInativacaoAsync(
            int coAcordo, SolicitarInativacaoDto dto, string matriculaSolicitante)
        {
            // Recupera o agregado e valida a pré-condição (deve estar Ativo)
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null || !acordo.PodeSolicitarInativacao()) return false;

            // Transição de estado: Ativo → PendenteInativacao
            acordo.CoSituacao = SituacaoAcordo.PendenteInativacao;
            acordo.DeJustificativaInativacao = dto.DeJustificativa;
            acordo.NuMatriculaGerenteAprovador = dto.NuMatriculaGerenteAprovador;

            // Registro imutável no histórico de auditoria (Audit Trail)
            acordo.Historicos.Add(new HistoricoAcordo
            {
                CoSituacaoAnterior = SituacaoAcordo.Ativo,
                CoSituacaoNova = SituacaoAcordo.PendenteInativacao,
                DeJustificativa = dto.DeJustificativa,
                NuMatriculaResponsavel = matriculaSolicitante,
                DtAlteracao = DateTime.UtcNow
            });

            // Persiste a transição de estado e o registro de auditoria
            await _repository.AtualizarAsync(acordo);
            return true;
        }

        /// <summary>
        /// Caso de uso: Avaliar (autorizar ou recusar) a inativação precoce.
        /// 
        /// Segundo passo do fluxo de dupla-custódia:
        /// o gestor com função gerencial decide se autoriza ou recusa
        /// a solicitação de inativação. Apenas função gerencial pode
        /// executar esta operação (requisito do PSI 15191).
        /// 
        /// Se aprovado: PendenteInativacao → Inativo
        /// Se recusado: PendenteInativacao → Ativo (retorna ao estado anterior)
        /// </summary>
        /// <param name="coAcordo">Identificador do Acordo.</param>
        /// <param name="dto">Decisão: aprovado (true) ou recusado (false).</param>
        /// <param name="matriculaGerente">Matrícula do gestor que avalia.</param>
        /// <returns>True se a avaliação foi registrada com sucesso.</returns>
        public async Task<bool> AvaliarInativacaoAsync(
            int coAcordo, AvaliarInativacaoDto dto, string matriculaGerente)
        {
            // Recupera o agregado e valida a pré-condição
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null || acordo.CoSituacao != SituacaoAcordo.PendenteInativacao)
                return false;

            // Determina o novo estado com base na decisão do gestor
            var novaSituacao = dto.Aprovado
                ? SituacaoAcordo.Inativo
                : SituacaoAcordo.Ativo;

            // Registro imutável da decisão no histórico de auditoria
            acordo.Historicos.Add(new HistoricoAcordo
            {
                CoSituacaoAnterior = SituacaoAcordo.PendenteInativacao,
                CoSituacaoNova = novaSituacao,
                DeJustificativa = dto.Aprovado ? "Inativação autorizada pelo gestor" : "Inativação recusada pelo gestor",
                NuMatriculaResponsavel = matriculaGerente,
                DtAlteracao = DateTime.UtcNow
            });

            // Aplica a transição de estado decidida pelo gestor
            acordo.CoSituacao = novaSituacao;
            await _repository.AtualizarAsync(acordo);
            return true;
        }

        /// <summary>
        /// Caso de uso: Excluir ANS (exclusão lógica / soft delete).
        /// 
        /// A exclusão só é permitida para ANS com situação "Pendente"
        /// (não assinados por ambas as partes). Acordos ativos ou
        /// inativos são preservados para fins de auditoria e histórico.
        /// 
        /// O registro nunca é removido fisicamente do banco de dados —
        /// sua situação é alterada para "Excluído" (soft delete).
        /// </summary>
        /// <param name="coAcordo">Identificador do Acordo.</param>
        /// <param name="matricula">Matrícula do empregado que solicita a exclusão.</param>
        /// <returns>True se a exclusão lógica foi realizada com sucesso.</returns>
        public async Task<bool> ExcluirAcordoAsync(int coAcordo, string matricula)
        {
            // Recupera o agregado e valida a pré-condição (deve estar Pendente)
            var acordo = await _repository.ObterPorIdAsync(coAcordo);
            if (acordo == null || !acordo.PodeExcluir()) return false;

            // Registro imutável da exclusão no histórico de auditoria
            acordo.Historicos.Add(new HistoricoAcordo
            {
                CoSituacaoAnterior = acordo.CoSituacao,
                CoSituacaoNova = SituacaoAcordo.Excluido,
                DeJustificativa = "Exclusão lógica solicitada pelo empregado GEGOD",
                NuMatriculaResponsavel = matricula,
                DtAlteracao = DateTime.UtcNow
            });

            // Executa a exclusão lógica via repositório
            await _repository.ExcluirLogicoAsync(coAcordo);
            return true;
        }
    }
}
