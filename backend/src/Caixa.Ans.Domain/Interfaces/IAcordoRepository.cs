// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Interface: Contrato que define quais operacoes o repositorio deve oferecer.
// - Repository Pattern: Camada que isola o acesso ao banco de dados.
// - Task: Operacao assincrona (nao bloqueia o sistema enquanto espera).
// ============================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Caixa.Ans.Domain.Entities;

namespace Caixa.Ans.Domain.Interfaces
{
    /// <summary>
    /// Contrato para o repositorio de Acordos.
    /// A implementacao concreta fica na camada de Infraestrutura.
    /// </summary>
    public interface IAcordoRepository
    {
        Task<Acordo?> ObterPorIdAsync(int coAcordo);
        Task<IEnumerable<Acordo>> ListarTodosAsync(string? filtro, string? situacao, string? ordenarPor);
        Task<Acordo> IncluirAsync(Acordo acordo);
        Task AtualizarAsync(Acordo acordo);
        Task ExcluirLogicoAsync(int coAcordo);
    }
}
