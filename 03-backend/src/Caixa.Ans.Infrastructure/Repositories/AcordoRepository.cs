// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Repository Pattern] Martin Fowler — PoEAA:
//   Implementação concreta que encapsula a lógica de acesso a dados
//   utilizando Entity Framework Core como ORM.
//
// [Eager Loading] EF Core — Include/ThenInclude:
//   Carregamento antecipado de entidades relacionadas para evitar
//   o problema N+1 (múltiplas queries para uma única consulta lógica).
//
// [LINQ to Entities] Composição de queries:
//   As consultas são construídas de forma composicional (IQueryable),
//   permitindo que filtros e ordenação sejam traduzidos em SQL
//   otimizado pelo provider do EF Core.
//
// [Soft Delete] Exclusão lógica:
//   Registros nunca são removidos fisicamente — a situação é
//   alterada para "Excluído", preservando integridade referencial.
// ============================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Caixa.Ans.Domain.Entities;
using Caixa.Ans.Domain.Enums;
using Caixa.Ans.Domain.Interfaces;
using Caixa.Ans.Infrastructure.Data;

namespace Caixa.Ans.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de Acordos utilizando
    /// Entity Framework Core como mecanismo de persistência.
    /// 
    /// Esta classe reside na camada de Infraestrutura e implementa
    /// a interface definida na camada de Domínio (IAcordoRepository),
    /// respeitando o Dependency Inversion Principle.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class AcordoRepository : IAcordoRepository
    {
        // DbContext injetado via construtor (Unit of Work do EF Core).
        private readonly AnsDbContext _context;

        /// <summary>
        /// Construtor com injeção do DbContext.
        /// O lifetime Scoped garante uma instância por requisição HTTP.
        /// </summary>
        public AcordoRepository(AnsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recupera um Acordo pelo identificador único, com eager loading
        /// de todas as entidades filhas (Responsáveis, Dados, Histórico).
        /// 
        /// O uso de Include() previne o problema N+1: todas as entidades
        /// relacionadas são carregadas em uma única query SQL com JOINs.
        /// </summary>
        /// <param name="coAcordo">Identificador único do Acordo.</param>
        /// <returns>Acordo completo ou null se não encontrado.</returns>
        public async Task<Acordo?> ObterPorIdAsync(int coAcordo)
        {
            return await _context.Acordos
                .Include(a => a.Responsaveis)        // Eager load: responsáveis técnicos e assinantes
                .Include(a => a.DadosCompartilhados)  // Eager load: ativos de informação
                .Include(a => a.Historicos)            // Eager load: trilha de auditoria
                .FirstOrDefaultAsync(a => a.CoAcordo == coAcordo);
        }

        /// <summary>
        /// Lista todos os Acordos com suporte a filtros composicionais.
        /// 
        /// A query é construída de forma incremental (IQueryable),
        /// permitindo que o EF Core traduza os filtros em cláusulas
        /// WHERE otimizadas no SQL Server. Os registros excluídos
        /// logicamente são automaticamente omitidos da listagem.
        /// 
        /// Atende ao requisito do PSI 15191: "filtrar resultados com
        /// base em palavras-chave que podem estar em qualquer campo
        /// do resultado ou pela situação do ANS".
        /// </summary>
        /// <param name="filtro">Texto livre para busca em múltiplos campos.</param>
        /// <param name="situacao">Filtro por situação do ANS.</param>
        /// <param name="ordenarPor">Campo de ordenação: coparticipada, vigencia ou criacao.</param>
        /// <returns>Coleção de Acordos que atendem aos critérios.</returns>
        public async Task<IEnumerable<Acordo>> ListarTodosAsync(
            string? filtro, string? situacao, string? ordenarPor)
        {
            // Query base: inclui responsáveis e exclui registros com soft delete
            var query = _context.Acordos
                .Include(a => a.Responsaveis)
                .Where(a => a.CoSituacao != SituacaoAcordo.Excluido)
                .AsQueryable();

            // Filtro por situação do ANS (quando informado)
            if (!string.IsNullOrEmpty(situacao))
            {
                if (System.Enum.TryParse<SituacaoAcordo>(situacao, true, out var sit))
                {
                    query = query.Where(a => a.CoSituacao == sit);
                }
            }

            // Filtro por palavras-chave em múltiplos campos (full-text search simplificado).
            // A busca case-insensitive é traduzida em LOWER() no SQL Server.
            if (!string.IsNullOrEmpty(filtro))
            {
                var termoLower = filtro.ToLower();
                query = query.Where(a =>
                    a.NoFornecedora.ToLower().Contains(termoLower) ||
                    a.NoConsumidora.ToLower().Contains(termoLower) ||
                    a.NoUnidadeFornecedora.ToLower().Contains(termoLower) ||
                    a.NoUnidadeConsumidora.ToLower().Contains(termoLower) ||
                    a.CoGrauSigilo.ToLower().Contains(termoLower));
            }

            // Ordenação dinâmica via switch expression (C# 8+).
            // Default: ordenação por data de criação decrescente (mais recentes primeiro).
            query = ordenarPor?.ToLower() switch
            {
                "coparticipada" => query.OrderBy(a => a.NoConsumidora),
                "vigencia" => query.OrderBy(a => a.DtFimVigencia),
                "criacao" => query.OrderByDescending(a => a.DtCriacao),
                _ => query.OrderByDescending(a => a.DtCriacao)
            };

            // Materializa a query e retorna a coleção
            return await query.ToListAsync();
        }

        /// <summary>
        /// Persiste um novo Acordo no banco de dados.
        /// O EF Core rastreia automaticamente as entidades filhas
        /// (Responsáveis, Dados) e as persiste em cascata.
        /// </summary>
        /// <param name="acordo">Entidade Acordo com todos os dados preenchidos.</param>
        /// <returns>Acordo persistido com identificador gerado pelo banco.</returns>
        public async Task<Acordo> IncluirAsync(Acordo acordo)
        {
            // Add() marca a entidade e suas filhas como Added no Change Tracker
            _context.Acordos.Add(acordo);

            // SaveChangesAsync() persiste todas as alterações em uma única transação
            await _context.SaveChangesAsync();
            return acordo;
        }

        /// <summary>
        /// Atualiza um Acordo existente no banco de dados.
        /// Utilizado para registrar assinaturas, transições de estado
        /// e novos registros de auditoria no histórico.
        /// </summary>
        /// <param name="acordo">Entidade Acordo com os dados atualizados.</param>
        public async Task AtualizarAsync(Acordo acordo)
        {
            // Update() marca a entidade como Modified no Change Tracker
            _context.Acordos.Update(acordo);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Executa a exclusão lógica (soft delete) de um Acordo.
        /// 
        /// O registro não é removido fisicamente — sua situação é
        /// alterada para "Excluído". Isso preserva a integridade
        /// referencial e permite auditoria posterior.
        /// 
        /// O filtro na listagem (Where CoSituacao != Excluido) garante
        /// que registros excluídos não apareçam para os usuários.
        /// </summary>
        /// <param name="coAcordo">Identificador do Acordo a ser excluído.</param>
        public async Task ExcluirLogicoAsync(int coAcordo)
        {
            // FindAsync() utiliza o cache do Change Tracker quando disponível
            var acordo = await _context.Acordos.FindAsync(coAcordo);

            if (acordo != null)
            {
                // Soft delete: altera a situação em vez de remover o registro
                acordo.CoSituacao = SituacaoAcordo.Excluido;
                await _context.SaveChangesAsync();
            }
        }
    }
}
