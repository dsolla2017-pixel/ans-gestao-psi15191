// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - DbContext: Classe que representa a conexao com o banco de dados.
// - DbSet: Representa uma tabela do banco dentro do codigo C#.
// - OnModelCreating: Metodo que define as regras de mapeamento (tabelas, colunas, chaves).
// - Fluent API: Forma de configurar o banco usando codigo ao inves de XML.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Caixa.Ans.Domain.Entities;

namespace Caixa.Ans.Infrastructure.Data
{
    /// <summary>
    /// Contexto do Entity Framework Core para o Sistema de Gestao de ANS.
    /// Mapeia as entidades do dominio para as tabelas do banco de dados,
    /// seguindo o Guia de Nomenclatura de Objetos da CAIXA.
    /// </summary>
    public class AnsDbContext : DbContext
    {
        public AnsDbContext(DbContextOptions<AnsDbContext> options) : base(options) { }

        public DbSet<Acordo> Acordos { get; set; } = null!;
        public DbSet<Responsavel> Responsaveis { get; set; } = null!;
        public DbSet<DadoCompartilhado> DadosCompartilhados { get; set; } = null!;
        public DbSet<HistoricoAcordo> Historicos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================================
            // TB_ANS_ACORDO (Tabela principal dos acordos)
            // ============================================================
            modelBuilder.Entity<Acordo>(entity =>
            {
                entity.ToTable("TB_ANS_ACORDO");
                entity.HasKey(e => e.CoAcordo);
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");
                entity.Property(e => e.NoFornecedora).HasColumnName("NO_FORNECEDORA").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NoConsumidora).HasColumnName("NO_CONSUMIDORA").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NoUnidadeFornecedora).HasColumnName("NO_UNIDADE_FORNECEDORA").HasMaxLength(200);
                entity.Property(e => e.NoUnidadeConsumidora).HasColumnName("NO_UNIDADE_CONSUMIDORA").HasMaxLength(200);
                entity.Property(e => e.QtDiasVigencia).HasColumnName("QT_DIAS_VIGENCIA");
                entity.Property(e => e.DtCriacao).HasColumnName("DT_CRIACAO");
                entity.Property(e => e.DtAssinatura).HasColumnName("DT_ASSINATURA");
                entity.Property(e => e.DtFimVigencia).HasColumnName("DT_FIM_VIGENCIA");
                entity.Property(e => e.CoSituacao).HasColumnName("CO_SITUACAO");
                entity.Property(e => e.CoGrauSigilo).HasColumnName("CO_GRAU_SIGILO").HasMaxLength(50);
                entity.Property(e => e.DePeriodicidade).HasColumnName("DE_PERIODICIDADE").HasMaxLength(100);
                entity.Property(e => e.IcDadoPessoal).HasColumnName("IC_DADO_PESSOAL");
                entity.Property(e => e.IcDadoSensivel).HasColumnName("IC_DADO_SENSIVEL");
                entity.Property(e => e.DeJustificativaInativacao).HasColumnName("DE_JUSTIFICATIVA_INATIVACAO").HasMaxLength(1000);
                entity.Property(e => e.NuMatriculaGerenteAprovador).HasColumnName("NU_MATRICULA_GERENTE_APROVADOR").HasMaxLength(20);
                entity.Property(e => e.NuMatriculaCriador).HasColumnName("NU_MATRICULA_CRIADOR").HasMaxLength(20);
                entity.Property(e => e.DtUltimaAlteracao).HasColumnName("DT_ULTIMA_ALTERACAO");

                // Indices para performance
                entity.HasIndex(e => e.CoSituacao).HasDatabaseName("IX_ANS_ACORDO_SITUACAO");
                entity.HasIndex(e => e.NoFornecedora).HasDatabaseName("IX_ANS_ACORDO_FORNECEDORA");
                entity.HasIndex(e => e.NoConsumidora).HasDatabaseName("IX_ANS_ACORDO_CONSUMIDORA");
                entity.HasIndex(e => e.DtFimVigencia).HasDatabaseName("IX_ANS_ACORDO_FIM_VIGENCIA");
            });

            // ============================================================
            // TB_ANS_RESPONSAVEL (Responsaveis tecnicos e assinantes)
            // ============================================================
            modelBuilder.Entity<Responsavel>(entity =>
            {
                entity.ToTable("TB_ANS_RESPONSAVEL");
                entity.HasKey(e => e.CoResponsavel);
                entity.Property(e => e.CoResponsavel).HasColumnName("CO_RESPONSAVEL");
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");
                entity.Property(e => e.NoPessoa).HasColumnName("NO_PESSOA").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NuCpf).HasColumnName("NU_CPF").HasMaxLength(11).IsRequired();
                entity.Property(e => e.NuMatricula).HasColumnName("NU_MATRICULA").HasMaxLength(20);
                entity.Property(e => e.NuTelefone).HasColumnName("NU_TELEFONE").HasMaxLength(20);
                entity.Property(e => e.DeEmail).HasColumnName("DE_EMAIL").HasMaxLength(200);
                entity.Property(e => e.NoCargoFuncao).HasColumnName("NO_CARGO_FUNCAO").HasMaxLength(100);
                entity.Property(e => e.TpPapel).HasColumnName("TP_PAPEL");
                entity.Property(e => e.DtAssinatura).HasColumnName("DT_ASSINATURA");

                entity.HasOne(e => e.Acordo)
                    .WithMany(a => a.Responsaveis)
                    .HasForeignKey(e => e.CoAcordo)
                    .HasConstraintName("FK_ANS_RESPONSAVEL_ACORDO");
            });

            // ============================================================
            // TB_ANS_DADO_COMPARTILHADO (Dados objeto do compartilhamento)
            // ============================================================
            modelBuilder.Entity<DadoCompartilhado>(entity =>
            {
                entity.ToTable("TB_ANS_DADO_COMPARTILHADO");
                entity.HasKey(e => e.CoDado);
                entity.Property(e => e.CoDado).HasColumnName("CO_DADO");
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");
                entity.Property(e => e.NoNegocial).HasColumnName("NO_NEGOCIAL").HasMaxLength(200).IsRequired();
                entity.Property(e => e.NoObjetoFisico).HasColumnName("NO_OBJETO_FISICO").HasMaxLength(200).IsRequired();
                entity.Property(e => e.DeLink).HasColumnName("DE_LINK").HasMaxLength(500);
                entity.Property(e => e.IcDadoPessoal).HasColumnName("IC_DADO_PESSOAL");
                entity.Property(e => e.IcDadoSensivel).HasColumnName("IC_DADO_SENSIVEL");

                entity.HasOne(e => e.Acordo)
                    .WithMany(a => a.DadosCompartilhados)
                    .HasForeignKey(e => e.CoAcordo)
                    .HasConstraintName("FK_ANS_DADO_COMPARTILHADO_ACORDO");
            });

            // ============================================================
            // TB_ANS_HISTORICO (Trilha de auditoria)
            // ============================================================
            modelBuilder.Entity<HistoricoAcordo>(entity =>
            {
                entity.ToTable("TB_ANS_HISTORICO");
                entity.HasKey(e => e.CoHistorico);
                entity.Property(e => e.CoHistorico).HasColumnName("CO_HISTORICO");
                entity.Property(e => e.CoAcordo).HasColumnName("CO_ACORDO");
                entity.Property(e => e.CoSituacaoAnterior).HasColumnName("CO_SITUACAO_ANTERIOR");
                entity.Property(e => e.CoSituacaoNova).HasColumnName("CO_SITUACAO_NOVA");
                entity.Property(e => e.DeJustificativa).HasColumnName("DE_JUSTIFICATIVA").HasMaxLength(1000);
                entity.Property(e => e.DtAlteracao).HasColumnName("DT_ALTERACAO");
                entity.Property(e => e.NuMatriculaResponsavel).HasColumnName("NU_MATRICULA_RESPONSAVEL").HasMaxLength(20);

                entity.HasOne(e => e.Acordo)
                    .WithMany(a => a.Historicos)
                    .HasForeignKey(e => e.CoAcordo)
                    .HasConstraintName("FK_ANS_HISTORICO_ACORDO");

                entity.HasIndex(e => e.DtAlteracao).HasDatabaseName("IX_ANS_HISTORICO_DT_ALTERACAO");
            });
        }
    }
}
