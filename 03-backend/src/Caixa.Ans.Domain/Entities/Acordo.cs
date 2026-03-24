// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [DDD] Domain-Driven Design (Eric Evans, 2003):
//   Esta entidade é o Aggregate Root do contexto de Acordos,
//   responsável por encapsular invariantes de negócio e garantir
//   consistência transacional dentro do agregado.
//
// [Rich Domain Model] Martin Fowler — Patterns of Enterprise
//   Application Architecture: As regras de negócio residem na
//   entidade (PodeAtivar, PodeExcluir, PodeSolicitarInativacao),
//   evitando o antipadrão Anemic Domain Model.
//
// [Nomenclatura CAIXA] Guia de Nomenclatura de Objetos SGBD:
//   Prefixos CO_ (código), NO_ (nome), DT_ (data), QT_ (quantidade),
//   IC_ (indicador), DE_ (descrição), NU_ (número), TB_ (tabela).
// ============================================================

using System;
using System.Collections.Generic;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Aggregate Root do domínio de Acordos de Nível de Serviço (ANS).
    /// Representa o contrato formal de compartilhamento de dados entre
    /// a CAIXA Econômica Federal e empresas do Conglomerado CAIXA.
    /// 
    /// Mapeada para a tabela TB_ANS_ACORDO conforme o Guia de Nomenclatura
    /// de Objetos do SGBD CAIXA.
    /// 
    /// Responsabilidades:
    /// - Encapsular as invariantes de negócio do ciclo de vida do ANS
    /// - Garantir transições de estado válidas (Pendente → Ativo → Inativo)
    /// - Proteger a integridade referencial com Responsáveis e Dados Compartilhados
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class Acordo
    {
        // ── Identificação ──────────────────────────────────────────
        // Chave primária surrogate (CO_ = código). Gerada pelo banco
        // de dados via IDENTITY, garantindo unicidade sem dependência
        // de regras de negócio (cf. Vaughn Vernon, Implementing DDD).
        public int CoAcordo { get; set; }

        // ── Partes Envolvidas ──────────────────────────────────────
        // NO_ = nome descritivo. Identifica as empresas do conglomerado
        // que participam do acordo de compartilhamento de dados.
        // A separação entre Fornecedora e Consumidora reflete o fluxo
        // direcional dos dados conforme o Acordo Operacional.
        public string NoFornecedora { get; set; } = string.Empty;
        public string NoConsumidora { get; set; } = string.Empty;

        // Unidades responsáveis dentro de cada empresa participante.
        // Granularidade organizacional necessária para rastreabilidade
        // e atribuição de responsabilidade conforme MN OR016.
        public string NoUnidadeFornecedora { get; set; } = string.Empty;
        public string NoUnidadeConsumidora { get; set; } = string.Empty;

        // ── Vigência ───────────────────────────────────────────────
        // QT_ = quantidade. Período em dias corridos de validade do ANS.
        // A vigência é calculada a partir da data de assinatura bilateral.
        public int QtDiasVigencia { get; set; }

        // DT_ = data. Registro temporal do ciclo de vida do acordo.
        // DtCriacao utiliza UTC para consistência em ambientes distribuídos.
        public DateTime DtCriacao { get; set; } = DateTime.UtcNow;

        // Data em que a segunda assinatura é registrada, ativando o ANS.
        // Nullable porque o acordo nasce no estado Pendente, sem assinatura.
        public DateTime? DtAssinatura { get; set; }

        // Data calculada: DtAssinatura + QtDiasVigencia.
        // Nullable pois só é definida após a ativação do acordo.
        public DateTime? DtFimVigencia { get; set; }

        // ── Situação (Máquina de Estados) ──────────────────────────
        // Implementa o padrão State Machine para controlar transições
        // válidas: Pendente → Ativo → Inativo | PendenteInativacao → Inativo.
        // O ANS é criado obrigatoriamente com situação "Pendente",
        // conforme especificação do desafio PSI 15191.
        public SituacaoAcordo CoSituacao { get; set; } = SituacaoAcordo.Pendente;

        // ── Classificação dos Dados (LGPD e MN OR016) ──────────────
        // Grau de sigilo conforme a Matriz Normativa OR016 da CAIXA.
        // Valores possíveis: Público, Interno, Restrito, Confidencial.
        public string CoGrauSigilo { get; set; } = string.Empty;

        // DE_ = descrição. Periodicidade de atualização dos dados
        // compartilhados (diária, semanal, mensal, sob demanda).
        public string DePeriodicidade { get; set; } = string.Empty;

        // IC_ = indicador booleano. Flags de conformidade com a LGPD
        // (Lei 13.709/2018). Essenciais para governança de dados e
        // para o futuro módulo de classificação automática via IA.
        public bool IcDadoPessoal { get; set; }
        public bool IcDadoSensivel { get; set; }

        // ── Inativação Precoce (Dupla-Custódia) ────────────────────
        // Implementa o princípio de Segregação de Funções (SoD):
        // o empregado solicita a inativação com justificativa, e um
        // gestor com função gerencial autoriza ou recusa.
        // Esse mecanismo atende ao requisito de dupla-custódia do PSI 15191.
        public string? DeJustificativaInativacao { get; set; }

        // NU_ = número. Matrícula do gestor que autoriza a inativação.
        // Nullable porque só é preenchido quando há solicitação de inativação.
        public string? NuMatriculaGerenteAprovador { get; set; }

        // ── Auditoria e Rastreabilidade ────────────────────────────
        // Campos de auditoria para rastreabilidade completa.
        // Em conformidade com as diretrizes de governança corporativa
        // da CAIXA e com os princípios de accountability da LGPD.
        public string NuMatriculaCriador { get; set; } = string.Empty;
        public DateTime? DtUltimaAlteracao { get; set; }

        // ── Relacionamentos (Navigation Properties) ────────────────
        // Composição do agregado conforme DDD: Responsáveis e Dados
        // Compartilhados são entidades filhas que só existem no contexto
        // de um Acordo. O Histórico registra cada transição de estado.
        // Inicializados como listas vazias para evitar NullReferenceException
        // (Defensive Programming — cf. Code Complete, Steve McConnell).
        public ICollection<Responsavel> Responsaveis { get; set; } = new List<Responsavel>();
        public ICollection<DadoCompartilhado> DadosCompartilhados { get; set; } = new List<DadoCompartilhado>();
        public ICollection<HistoricoAcordo> Historicos { get; set; } = new List<HistoricoAcordo>();

        // ============================================================
        // REGRAS DE NEGÓCIO — Encapsuladas no Aggregate Root
        // Referência: Domain-Driven Design (Eric Evans), Cap. 6
        // "Aggregates enforce invariants for all data changes."
        // ============================================================

        /// <summary>
        /// Verifica se o acordo atende às pré-condições para ativação.
        /// 
        /// Regra de negócio: O ANS só pode ser ativado quando ambas as
        /// partes (Fornecedora e Consumidora) registrarem suas assinaturas.
        /// Essa regra implementa o requisito de consenso bilateral do PSI 15191.
        /// 
        /// Padrão aplicado: Guard Clause (Martin Fowler — Refactoring).
        /// A validação ocorre na entidade, não no serviço, garantindo que
        /// nenhum caminho de código consiga ativar um acordo sem as duas assinaturas.
        /// </summary>
        /// <returns>True se ambas as assinaturas estão presentes e o acordo está Pendente.</returns>
        public bool PodeAtivar()
        {
            // Contabiliza assinaturas válidas por papel (Fornecedora e Consumidora)
            var assinaturas = 0;
            foreach (var resp in Responsaveis)
            {
                // Verifica assinatura da parte Fornecedora dos dados
                if (resp.TpPapel == TipoPapel.AssinanteFornecedora && resp.DtAssinatura.HasValue)
                    assinaturas++;

                // Verifica assinatura da parte Consumidora dos dados
                if (resp.TpPapel == TipoPapel.AssinanteConsumidora && resp.DtAssinatura.HasValue)
                    assinaturas++;
            }

            // Pré-condição: duas assinaturas + estado Pendente
            return assinaturas >= 2 && CoSituacao == SituacaoAcordo.Pendente;
        }

        /// <summary>
        /// Verifica se o acordo pode ser excluído (exclusão lógica).
        /// 
        /// Regra de negócio: A exclusão só é permitida para ANS que ainda
        /// não foram assinados por ambas as partes. Acordos ativos ou
        /// inativos são preservados para fins de auditoria e histórico.
        /// 
        /// Padrão aplicado: Soft Delete — o registro nunca é removido
        /// fisicamente do banco de dados, apenas marcado como Excluído.
        /// Essa abordagem preserva a integridade referencial e atende
        /// aos requisitos de retenção de dados da CAIXA.
        /// </summary>
        /// <returns>True se o acordo está no estado Pendente (não assinado bilateralmente).</returns>
        public bool PodeExcluir()
        {
            // Apenas acordos pendentes podem ser excluídos logicamente
            return CoSituacao == SituacaoAcordo.Pendente;
        }

        /// <summary>
        /// Verifica se o acordo pode receber solicitação de inativação precoce.
        /// 
        /// Regra de negócio: Apenas acordos ativos podem ser inativados
        /// antes do fim da vigência. O processo exige justificativa do
        /// solicitante e autorização de um gestor com função gerencial
        /// (dupla-custódia), conforme requisito do PSI 15191.
        /// 
        /// Padrão aplicado: Segregação de Funções (Separation of Duties)
        /// — princípio de controle interno que impede que uma única pessoa
        /// execute todas as etapas de um processo crítico.
        /// </summary>
        /// <returns>True se o acordo está no estado Ativo.</returns>
        public bool PodeSolicitarInativacao()
        {
            // Apenas acordos com situação Ativo podem receber solicitação
            return CoSituacao == SituacaoAcordo.Ativo;
        }
    }
}
