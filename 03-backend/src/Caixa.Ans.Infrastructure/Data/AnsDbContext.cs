// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Unit of Work] Martin Fowler — PoEAA:
//   O DbContext implementa o padrão Unit of Work nativamente,
//   rastreando alterações e persistindo em uma única transação.
//
// [Fluent API] EF Core Configuration:
//   Mapeamento objeto-relacional via Fluent API (preferível a
//   Data Annotations para separação de responsabilidades).
//   As entidades de domínio permanecem livres de atributos de ORM.
//
// [Nomenclatura CAIXA] Guia de Nomenclatura de Objetos SGBD:
//   Tabelas: TB_ (tabela), Colunas: CO_ (código), NO_ (nome),
//   DT_ (data), NU_ (número), IC_ (indicador), DE_ (descrição),
//   QT_ (quantidade), TP_ (tipo). Índices: IX_, FKs: FK_.
//
// [Performance] Índices estratégicos nas colunas de filtro e
//   ordenação mais utilizadas (situação, fornecedora, consumidora,
//   vigência e data de alteração do histórico).
// ============================================================

using Microsoft.EntityFrameworkCore;
using Caixa.Ans.Domain.Entities;

namespace Caixa.Ans.Infrastructure.Data
{
    /// <summary>
    /// Contexto do Entity Framework Core para o Sistema de Gestão de ANS.
    /// 
    /// Responsabilidades:
    /// 1. Mapeia as entidades do domínio para tabelas do SQL Server
    /// 2. Implementa o padrão Unit of Work (rastreamento de alterações)
    /// 3. Aplica a nomenclatura de objetos conforme padrão CAIXA
    /// 4. Define índices estratégicos para performance de consultas
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AnsDbContext : DbContext
    {
        /// <summary>
        /// Construtor que recebe as opções de configuração (connection string, provider).
        /// A configuração é injetada pelo container de DI do ASP.NET Core.
        /// </summary>
        public AnsDbContext(DbContextOptions<AnsDbContext> options) : base(options) { }

        // ── DbSets (representação das tabelas no código C#) ────────
        // Cada DbSet corresponde a uma tabela no banco de dados.
        // O EF Core utiliza esses DbSets para gerar consultas SQL.

        /// <summary>Tabela TB_ANS_ACORDO — Acordos de Nível de Serviço.</summary>
        public DbSet<Acordo> Acordos { get; set; } = null!;

        /// <summary>Tabela TB_ANS_RESPONSAVEL — Responsáveis técnicos e assinantes.</summary>
        public DbSet<Responsavel> Responsaveis { get; set; } = null!;

        /// <summary>Tabela TB_ANS_DADO_COMPARTILHADO — Ativos de informação compartilhados.</summary>
        public DbSet<DadoCompartilhado> DadosCompartilhados { get; set; } = null!;

        /// <summary>Tabela TB_ANS_HISTORICO — Trilha de auditoria das transições de estado.</summary>
        public DbSet<HistoricoAcordo> Historicos { get; set; } = null!;

        /// <summary>
        /// Configuração do modelo relacional via Fluent API.
        /// 
        /// Preferência por Fluent API em vez de Data Annotations para:
        /// - Manter as entidades de domínio livres de atributos de ORM
        /// - Centralizar toda a configuração de persistência neste ponto
        /// - Facilitar a manutenção e evolução do schema
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ════════════════════════════════════════════════════════
            // TB_ANS_ACORDO — Tabela principal dos Acordos de Nível de Serviço
            // Aggregate Root do modelo de domínio
            // ════════════════════════════════════════════════════════
            modelBuilder.Entity<Acordo>(entity =>
            {
                // Mapeamento para tabela com nomenclatura CAIXA
                entity.ToTable("TB_ANS_ACORDO");
                entity.HasKey(e => e.CoAcordo);

                // Colunas de identificação
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");

                // Partes envolvidas — campos obrigatórios com limite de 200 caracteres
                entity.Property(e => e.NoFornecedora).HasColumnName("NO_FORNECEDORA").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NoConsumidora).HasColumnName("NO_CONSUMIDORA").HasMaxLength(200).IsRequired();

                // Unidades organizacionais responsáveis
                entity.Property(e => e.NoUnidadeFornecedora).HasColumnName("NO_UNIDADE_FORNECEDORA").HasMaxLength(200);
                entity.Property(e => e.NoUnidadeConsumidora).HasColumnName("NO_UNIDADE_CONSUMIDORA").HasMaxLength(200);

                // Vigência e datas do ciclo de vida
                entity.Property(e => e.QtDiasVigencia).HasColumnName("QT_DIAS_VIGENCIA");
                entity.Property(e => e.DtCriacao).HasColumnName("DT_CRIACAO");
                entity.Property(e => e.DtAssinatura).HasColumnName("DT_ASSINATURA");
                entity.Property(e => e.DtFimVigencia).HasColumnName("DT_FIM_VIGENCIA");

                // Situação atual (enum mapeado como inteiro)
                entity.Property(e => e.CoSituacao).HasColumnName("CO_SITUACAO");

                // Classificação de dados (sigilo conforme MN OR016)
                entity.Property(e => e.CoGrauSigilo).HasColumnName("CO_GRAU_SIGILO").HasMaxLength(50);
                entity.Property(e => e.DePeriodicidade).HasColumnName("DE_PERIODICIDADE").HasMaxLength(100);

                // Indicadores LGPD (Privacy by Design)
                entity.Property(e => e.IcDadoPessoal).HasColumnName("IC_DADO_PESSOAL");
                entity.Property(e => e.IcDadoSensivel).HasColumnName("IC_DADO_SENSIVEL");

                // Campos de inativação precoce (dupla-custódia)
                entity.Property(e => e.DeJustificativaInativacao).HasColumnName("DE_JUSTIFICATIVA_INATIVACAO").HasMaxLength(1000);
                entity.Property(e => e.NuMatriculaGerenteAprovador).HasColumnName("NU_MATRICULA_GERENTE_APROVADOR").HasMaxLength(20);

                // Rastreabilidade do criador
                entity.Property(e => e.NuMatriculaCriador).HasColumnName("NU_MATRICULA_CRIADOR").HasMaxLength(20);
                entity.Property(e => e.DtUltimaAlteracao).HasColumnName("DT_ULTIMA_ALTERACAO");

                // Índices estratégicos para performance das consultas mais frequentes
                entity.HasIndex(e => e.CoSituacao).HasDatabaseName("IX_ANS_ACORDO_SITUACAO");
                entity.HasIndex(e => e.NoFornecedora).HasDatabaseName("IX_ANS_ACORDO_FORNECEDORA");
                entity.HasIndex(e => e.NoConsumidora).HasDatabaseName("IX_ANS_ACORDO_CONSUMIDORA");
                entity.HasIndex(e => e.DtFimVigencia).HasDatabaseName("IX_ANS_ACORDO_FIM_VIGENCIA");
            });

            // ════════════════════════════════════════════════════════
            // TB_ANS_RESPONSAVEL — Responsáveis técnicos e assinantes
            // Entidade filha do Aggregate Acordo
            // ════════════════════════════════════════════════════════
            modelBuilder.Entity<Responsavel>(entity =>
            {
                entity.ToTable("TB_ANS_RESPONSAVEL");
                entity.HasKey(e => e.CoResponsavel);

                // Colunas de identificação
                entity.Property(e => e.CoResponsavel).HasColumnName("CO_RESPONSAVEL");
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");

                // Dados pessoais do responsável (CPF obrigatório, matrícula condicional)
                entity.Property(e => e.NoPessoa).HasColumnName("NO_PESSOA").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NuCpf).HasColumnName("NU_CPF").HasMaxLength(11).IsRequired();
                entity.Property(e => e.NuMatricula).HasColumnName("NU_MATRICULA").HasMaxLength(20);
                entity.Property(e => e.NuTelefone).HasColumnName("NU_TELEFONE").HasMaxLength(20);
                entity.Property(e => e.DeEmail).HasColumnName("DE_EMAIL").HasMaxLength(200);
                entity.Property(e => e.NoCargoFuncao).HasColumnName("NO_CARGO_FUNCAO").HasMaxLength(100);

                // Papel no acordo (enum) e data da assinatura
                entity.Property(e => e.TpPapel).HasColumnName("TP_PAPEL");
                entity.Property(e => e.DtAssinatura).HasColumnName("DT_ASSINATURA");

                // Relacionamento N:1 com Acordo (Foreign Key explícita)
                entity.HasOne(e => e.Acordo)
                    .WithMany(a => a.Responsaveis)
                    .HasForeignKey(e => e.CoAcordo)
                    .HasConstraintName("FK_ANS_RESPONSAVEL_ACORDO");
            });

            // ════════════════════════════════════════════════════════
            // TB_ANS_DADO_COMPARTILHADO — Ativos de informação
            // Entidade filha do Aggregate Acordo
            // ════════════════════════════════════════════════════════
            modelBuilder.Entity<DadoCompartilhado>(entity =>
            {
                entity.ToTable("TB_ANS_DADO_COMPARTILHADO");
                entity.HasKey(e => e.CoDado);

                // Colunas de identificação
                entity.Property(e => e.CoDado).HasColumnName("CO_DADO");
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");

                // Descrição do ativo de informação
                entity.Property(e => e.NoNegocial).HasColumnName("NO_NEGOCIAL").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NoObjetoFisico).HasColumnName("NO_OBJETO_FISICO").HasMaxLength(200).IsRequired();
                entity.Property(e => e.DeLink).HasColumnName("DE_LINK").HasMaxLength(500);

                // Classificação LGPD no nível do dado individual
                entity.Property(e => e.IcDadoPessoal).HasColumnName("IC_DADO_PESSOAL");
                entity.Property(e => e.IcDadoSensivel).HasColumnName("IC_DADO_SENSIVEL");

                // Relacionamento N:1 com Acordo
                entity.HasOne(e => e.Acordo)
                    .WithMany(a => a.DadosCompartilhados)
                    .HasForeignKey(e => e.CoAcordo)
                    .HasConstraintName("FK_ANS_DADO_COMPARTILHADO_ACORDO");
            });

            // ════════════════════════════════════════════════════════
            // TB_ANS_HISTORICO — Trilha de auditoria (append-only)
            // Entidade filha do Aggregate Acordo
            // ════════════════════════════════════════════════════════
            modelBuilder.Entity<HistoricoAcordo>(entity =>
            {
                entity.ToTable("TB_ANS_HISTORICO");
                entity.HasKey(e => e.CoHistorico);

                // Colunas de identificação
                entity.Property(e => e.CoHistorico).HasColumnName("CO_HISTORICO");
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");

                // Transição de estado (anterior → nova)
                entity.Property(e => e.CoSituacaoAnterior).HasColumnName("CO_SITUACAO_ANTERIOR");
                entity.Property(e => e.CoSituacaoNova).HasColumnName("CO_SITUACAO_NOVA");

                // Contexto da alteração
                entity.Property(e => e.DeJustificativa).HasColumnName("DE_JUSTIFICATIVA").HasMaxLength(1000);
                entity.Property(e => e.DtAlteracao).HasColumnName("DT_ALTERACAO");
                entity.Property(e => e.NuMatriculaResponsavel).HasColumnName("NU_MATRICULA_RESPONSAVEL").HasMaxLength(20);

                // Relacionamento N:1 com Acordo
                entity.HasOne(e => e.Acordo)
                    .WithMany(a => a.Historicos)
                    .HasForeignKey(e => e.CoAcordo)
                    .HasConstraintName("FK_ANS_HISTORICO_ACORDO");

                // Índice para consultas de auditoria ordenadas por data
                entity.HasIndex(e => e.DtAlteracao).HasDatabaseName("IX_ANS_HISTORICO_DT_ALTERACAO");
            });
        }
    }
}
