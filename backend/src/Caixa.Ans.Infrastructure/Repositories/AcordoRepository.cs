// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Repository: Classe que executa as operacoes no banco de dados.
// - LINQ: Linguagem integrada ao C# para consultar dados de forma elegante.
// - Include: Comando que carrega dados relacionados (ex: responsaveis do acordo).
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
    /// Implementacao concreta do repositorio de Acordos.
    /// Utiliza Entity Framework Core para acesso ao banco de dados.
    /// </summary>
    public class AcordoRepository : IAcordoRepository
    {
        private readonly AnsDbContext _context;

        public AcordoRepository(AnsDbContext context)
        {
            _context = context;
        }

        public async Task<Acordo?> ObterPorIdAsync(int coAcordo)
        {
            return await _context.Acordos
                .Include(a => a.Responsaveis)
                .Include(a => a.DadosCompartilhados)
                .Include(a => a.Historicos)
                .FirstOrDefaultAsync(a => a.CoAcordo == coAcordo);
        }

        public async Task<IEnumerable<Acordo>> ListarTodosAsync(
            string? filtro, string? situacao, string? ordenarPor)
        {
            var query = _context.Acordos
                .Include(a => a.Responsaveis)
                .Where(a => a.CoSituacao != SituacaoAcordo.Excluido)
                .AsQueryable();

            // Filtro por situacao
            if (!string.IsNullOrEmpty(situacao))
            {
                if (System.Enum.TryParse<SituacaoAcordo>(situacao, true, out var sit))
                {
                    query = query.Where(a => a.CoSituacao == sit);
                }
            }

            // Filtro por palavras-chave (busca em multiplos campos)
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

            // Ordenacao
            query = ordenarPor?.ToLower() switch
            {
                "coparticipada" => query.OrderBy(a => a.NoConsumidora),
                "vigencia" => query.OrderBy(a => a.DtFimVigencia),
                "criacao" => query.OrderByDescending(a => a.DtCriacao),
                _ => query.OrderByDescending(a => a.DtCriacao)
            };

            return await query.ToListAsync();
        }

        public async Task<Acordo> IncluirAsync(Acordo acordo)
        {
            _context.Acordos.Add(acordo);
            await _context.SaveChangesAsync();
            return acordo;
        }

        public async Task AtualizarAsync(Acordo acordo)
        {
            _context.Acordos.Update(acordo);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirLogicoAsync(int coAcordo)
        {
            var acordo = await _context.Acordos.FindAsync(coAcordo);
            if (acordo != null)
            {
                acordo.CoSituacao = SituacaoAcordo.Excluido;
                await _context.SaveChangesAsync();
            }
        }
    }
}
