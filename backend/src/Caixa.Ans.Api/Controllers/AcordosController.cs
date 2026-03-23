// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Controller: Ponto de entrada da API que recebe requisicoes HTTP.
// - Endpoint: Endereco (URL) que o front-end chama para obter ou enviar dados.
// - [Authorize]: Atributo que exige autenticacao para acessar o recurso.
// - ActionResult: Tipo de retorno que permite enviar codigos HTTP (200, 404 etc.).
// ============================================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Caixa.Ans.Application.DTOs;
using Caixa.Ans.Application.Services;

namespace Caixa.Ans.Api.Controllers
{
    /// <summary>
    /// Controller RESTful para gerenciamento de Acordos de Nivel de Servico.
    /// Base URL: /api/v1/acordos
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AcordosController : ControllerBase
    {
        private readonly AcordoService _service;

        public AcordosController(AcordoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todos os ANS com filtros opcionais.
        /// GET /api/v1/acordos?filtro=texto&situacao=Ativo&ordenarPor=vigencia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] string? filtro,
            [FromQuery] string? situacao,
            [FromQuery] string? ordenarPor)
        {
            var resultado = await _service.ListarAcordosAsync(filtro, situacao, ordenarPor);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtem os detalhes completos de um ANS.
        /// GET /api/v1/acordos/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterDetalhes(int id)
        {
            var acordo = await _service.ObterDetalheAsync(id);
            if (acordo == null)
                return NotFound(new { mensagem = "Acordo nao encontrado." });

            return Ok(acordo);
        }

        /// <summary>
        /// Cria um novo ANS com situacao Pendente.
        /// POST /api/v1/acordos
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarAcordoDto dto)
        {
            // Em producao, a matricula vem do token JWT
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var resultado = await _service.CriarAcordoAsync(dto, matricula);
            return CreatedAtAction(nameof(ObterDetalhes),
                new { id = resultado.CoAcordo }, resultado);
        }

        /// <summary>
        /// Registra a assinatura de uma das partes.
        /// PATCH /api/v1/acordos/{id}/assinar
        /// </summary>
        [HttpPatch("{id}/assinar")]
        public async Task<IActionResult> Assinar(int id, [FromQuery] string papel)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.AssinarAcordoAsync(id, matricula, papel);
            if (!sucesso)
                return BadRequest(new { mensagem = "Nao foi possivel assinar o acordo." });

            return Ok(new { mensagem = "Assinatura registrada com sucesso." });
        }

        /// <summary>
        /// Solicita a inativacao precoce (dupla-custodia).
        /// POST /api/v1/acordos/{id}/solicitar-inativacao
        /// </summary>
        [HttpPost("{id}/solicitar-inativacao")]
        public async Task<IActionResult> SolicitarInativacao(
            int id, [FromBody] SolicitarInativacaoDto dto)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.SolicitarInativacaoAsync(id, dto, matricula);
            if (!sucesso)
                return BadRequest(new { mensagem = "Nao foi possivel solicitar a inativacao." });

            return Ok(new { mensagem = "Solicitacao de inativacao registrada." });
        }

        /// <summary>
        /// Gerente avalia (aprova/recusa) a inativacao.
        /// PATCH /api/v1/acordos/{id}/avaliar-inativacao
        /// </summary>
        [HttpPatch("{id}/avaliar-inativacao")]
        public async Task<IActionResult> AvaliarInativacao(
            int id, [FromBody] AvaliarInativacaoDto dto)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.AvaliarInativacaoAsync(id, dto, matricula);
            if (!sucesso)
                return BadRequest(new { mensagem = "Nao foi possivel avaliar a inativacao." });

            return Ok(new { mensagem = dto.Aprovado ? "Inativacao aprovada." : "Inativacao recusada." });
        }

        /// <summary>
        /// Exclui logicamente um ANS (somente se pendente).
        /// DELETE /api/v1/acordos/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var matricula = User?.Identity?.Name ?? "SISTEMA";
            var sucesso = await _service.ExcluirAcordoAsync(id, matricula);
            if (!sucesso)
                return BadRequest(new { mensagem = "Nao foi possivel excluir o acordo." });

            return Ok(new { mensagem = "Acordo excluido logicamente." });
        }
    }
}
