// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Repository Pattern] Martin Fowler — Patterns of Enterprise
//   Application Architecture: Abstrai o acesso a dados por trás
//   de uma interface orientada a coleções, isolando o domínio
//   de detalhes de persistência (EF Core, Dapper, ADO.NET).
//
// [Dependency Inversion Principle] SOLID — Robert C. Martin:
//   A camada de Domínio define a interface; a camada de
//   Infraestrutura fornece a implementação concreta.
//   O domínio nunca depende de frameworks de persistência.
//
// [Async/Await] TAP — Task-based Asynchronous Pattern:
//   Todas as operações são assíncronas para não bloquear threads
//   do servidor durante operações de I/O com o banco de dados.
// ============================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Caixa.Ans.Domain.Entities;

namespace Caixa.Ans.Domain.Interfaces
{
    /// <summary>
    /// Contrato que define as operações de persistência para o
    /// Aggregate Root Acordo. A implementação concreta reside na
    /// camada de Infraestrutura (Caixa.Ans.Infrastructure).
    /// 
    /// Este contrato segue o Repository Pattern (Fowler, 2002) e o
    /// Dependency Inversion Principle (Martin, 2003), garantindo que
    /// o domínio permaneça independente de frameworks de persistência.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public interface IAcordoRepository
    {
        /// <summary>
        /// Recupera um Acordo pelo seu identificador único, incluindo
        /// todas as entidades filhas (Responsáveis, Dados, Histórico).
        /// Retorna null quando o identificador não corresponde a nenhum registro.
        /// </summary>
        /// <param name="coAcordo">Identificador único do Acordo (chave primária).</param>
        Task<Acordo?> ObterPorIdAsync(int coAcordo);

        /// <summary>
        /// Lista todos os Acordos com suporte a filtro por palavras-chave,
        /// situação e ordenação. Atende ao requisito do PSI 15191 de
        /// "filtrar resultados com base em palavras-chave que podem estar
        /// em qualquer campo do resultado ou pela situação do ANS".
        /// </summary>
        /// <param name="filtro">Texto livre para busca em qualquer campo (nullable).</param>
        /// <param name="situacao">Filtro por situação do ANS: Pendente, Ativo, Inativo (nullable).</param>
        /// <param name="ordenarPor">Campo de ordenação: coparticipada ou data de vigência (nullable).</param>
        Task<IEnumerable<Acordo>> ListarTodosAsync(string? filtro, string? situacao, string? ordenarPor);

        /// <summary>
        /// Persiste um novo Acordo no banco de dados.
        /// O ANS é criado obrigatoriamente com situação "Pendente",
        /// conforme especificação do PSI 15191.
        /// </summary>
        /// <param name="acordo">Entidade Acordo com todos os dados preenchidos.</param>
        /// <returns>Acordo persistido com o identificador gerado pelo banco.</returns>
        Task<Acordo> IncluirAsync(Acordo acordo);

        /// <summary>
        /// Atualiza um Acordo existente no banco de dados.
        /// Utilizado para registrar assinaturas, alterar situação
        /// e incluir justificativas de inativação.
        /// </summary>
        /// <param name="acordo">Entidade Acordo com os dados atualizados.</param>
        Task AtualizarAsync(Acordo acordo);

        /// <summary>
        /// Executa a exclusão lógica (soft delete) de um Acordo.
        /// O registro não é removido fisicamente — sua situação é
        /// alterada para "Excluído", preservando a integridade
        /// referencial e o histórico de auditoria.
        /// </summary>
        /// <param name="coAcordo">Identificador único do Acordo a ser excluído.</param>
        Task ExcluirLogicoAsync(int coAcordo);
    }
}
