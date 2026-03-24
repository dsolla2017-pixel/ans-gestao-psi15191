// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [RESTful API Design] Richardson Maturity Model — Nível 2:
//   Cada recurso possui URL própria (/api/v1/acordos) e utiliza
//   verbos HTTP semânticos (GET, POST, PATCH, DELETE).
//
// [API Versioning] Versionamento explícito na URL (v1) para
//   garantir retrocompatibilidade em evoluções futuras.
//
// [Thin Controller] O controller delega toda a lógica ao
//   Application Service, mantendo-se como orquestrador HTTP.
//   Nenhuma regra de negócio reside nesta camada.
//
// [OWASP API Security Top 10] Autenticação via JWT, validação
//   de entrada via Data Annotations e tratamento global de erros.
// ============================================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Caixa.Ans.Application.DTOs;
using Caixa.Ans.Application.Services;

namespace Caixa.Ans.Api.Controllers
{
    /// <summary>
    /// Controller RESTful para gerenciamento de Acordos de Nível de Serviço.
    /// 
    /// Endpoints disponíveis:
    /// - GET    /api/v1/acordos                        → Listar ANS
    /// - GET    /api/v1/acordos/{id}                   → Consultar detalhes
    /// - POST   /api/v1/acordos                        → Incluir novo ANS
    /// - PATCH  /api/v1/acordos/{id}/assinar           → Registrar assinatura
    /// - POST   /api/v1/acordos/{id}/solicitar-inativacao → Solicitar inativação
    /// - PATCH  /api/v1/acordos/{id}/avaliar-inativacao   → Avaliar inativação
    /// - DELETE /api/v1/acordos/{id}                   → Excluir ANS
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AcordosController : ControllerBase
    {
        // Serviço de aplicação injetado via DI (Dependency Inversion Principle).
        private readonly AcordoService _service;

        /// <summary>
        /// Construtor com injeção de dependência do serviço de aplicação.
        /// O ASP.NET Core resolve automaticamente a instância registrada.
        /// </summary>
        public AcordosController(AcordoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todos os ANS com filtros opcionais.
        /// 
        /// Parâmetros de query string:
        /// - filtro: texto livre para busca em qualquer campo
        /// - situacao: Pendente, Ativo, Inativo, PendenteInativacao
        /// - ordenarPor: coparticipada ou vigencia
        /// 
        /// Retorna HTTP 200 com a coleção de acordos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] string? filtro,
            [FromQuery] string? situacao,
            [FromQuery] string? ordenarPor)
        {
            // Delega ao serviço de aplicação e retorna a coleção projetada
            var resultado = await _service.ListarAcordosAsync(filtro, situacao, ordenarPor);
            return Ok(resultado);
        }

        /// <summary>
        /// Consulta os detalhes completos de um ANS específico.
        /// 
        /// Retorna HTTP 200 com o detalhamento ou HTTP 404 se não encontrado.
        /// Inclui responsáveis, dados compartilhados e classificação LGPD.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterDetalhes(int id)
        {
            var acordo = await _service.ObterDetalheAsync(id);

            // Guard Clause: retorna 404 com mensagem descritiva
            if (acordo == null)
                return NotFound(new { mensagem = "Acordo não encontrado." });

            return Ok(acordo);
        }

        /// <summary>
        /// Inclui um novo ANS com situação "Pendente" (invariante obrigatória).
        /// 
        /// Recebe o DTO do formulário Angular e retorna HTTP 201 (Created)
        /// com o header Location apontando para o recurso criado.
        /// Padrão REST: CreatedAtAction para HATEOAS básico.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "EmpregadoGEGOD")]
        public async Task<IActionResult> Criar([FromBody] CriarAcordoDto dto)
        {
            // Em produção, a matrícula é extraída do token JWT autenticado.
            // O fallback "SISTEMA" é utilizado apenas em ambiente de desenvolvimento.
            var matricula = User?.Identity?.Name ?? "SISTEMA";

            var resultado = await _service.CriarAcordoAsync(dto, matricula);

            // HTTP 201 Created com header Location para o recurso criado
            return CreatedAtAction(nameof(ObterDetalhes),
                new { id = resultado.CoAcordo }, resultado);
        }

        /// <summary>
        /// Registra a assinatura de uma das partes (Fornecedora ou Consumidora).
        /// 
        /// Utiliza PATCH (atualização parcial) conforme semântica REST.
        /// Quando ambas as partes assinam, o ANS é ativado automaticamente
        /// pela regra de negócio no Application Service.
        /// </summary>
        [HttpPatch("{id}/assinar")]
        [Authorize(Policy = "EmpregadoGEGOD")]
        public async Task<IActionResult> Assinar(int id, [FromQuery] string papel)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.AssinarAcordoAsync(id, matricula, papel);

            if (!sucesso)
                return BadRequest(new { mensagem = "Não foi possível registrar a assinatura." });

            return Ok(new { mensagem = "Assinatura registrada com sucesso." });
        }

        /// <summary>
        /// Solicita a inativação precoce de um ANS (primeiro passo da dupla-custódia).
        /// 
        /// O empregado GEGOD fornece a justificativa e indica o gestor aprovador.
        /// O ANS transita de "Ativo" para "PendenteInativacao".
        /// </summary>
        [HttpPost("{id}/solicitar-inativacao")]
        [Authorize(Policy = "EmpregadoGEGOD")]
        public async Task<IActionResult> SolicitarInativacao(
            int id, [FromBody] SolicitarInativacaoDto dto)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.SolicitarInativacaoAsync(id, dto, matricula);

            if (!sucesso)
                return BadRequest(new { mensagem = "Não foi possível solicitar a inativação." });

            return Ok(new { mensagem = "Solicitação de inativação registrada com sucesso." });
        }

        /// <summary>
        /// Avalia (autoriza ou recusa) a inativação precoce (segundo passo da dupla-custódia).
        /// 
        /// Apenas gestores com função gerencial na GEGOD podem executar esta operação.
        /// Se aprovado: PendenteInativacao → Inativo.
        /// Se recusado: PendenteInativacao → Ativo.
        /// </summary>
        [HttpPatch("{id}/avaliar-inativacao")]
        [Authorize(Policy = "GerenteGEGOD")]
        public async Task<IActionResult> AvaliarInativacao(
            int id, [FromBody] AvaliarInativacaoDto dto)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.AvaliarInativacaoAsync(id, dto, matricula);

            if (!sucesso)
                return BadRequest(new { mensagem = "Não foi possível avaliar a inativação." });

            return Ok(new { mensagem = dto.Aprovado ? "Inativação autorizada." : "Inativação recusada." });
        }

        /// <summary>
        /// Exclui logicamente um ANS (soft delete).
        /// 
        /// A exclusão só é permitida para ANS com situação "Pendente".
        /// Acordos já assinados são preservados para fins de auditoria.
        /// O registro não é removido fisicamente — apenas sua situação
        /// é alterada para "Excluído".
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "EmpregadoGEGOD")]
        public async Task<IActionResult> Excluir(int id)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.ExcluirAcordoAsync(id, matricula);

            if (!sucesso)
                return BadRequest(new { mensagem = "Não foi possível excluir o acordo." });

            return Ok(new { mensagem = "Acordo excluído logicamente com sucesso." });
        }
    }
}
