// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [Middleware Pipeline] ASP.NET Core — Request Pipeline:
//   Componente que intercepta todas as requisições HTTP antes
//   de alcançar os controllers, implementando o padrão Chain
//   of Responsibility (GoF) para tratamento transversal.
//
// [RFC 7807] Problem Details for HTTP APIs:
//   Formato padronizado para respostas de erro em APIs REST.
//   Garante interoperabilidade entre front-end e back-end.
//
// [OWASP] Information Disclosure Prevention:
//   A stack trace e detalhes internos da exceção são registrados
//   apenas no log do servidor — nunca expostos ao cliente.
//   Isso previne vazamento de informações sensíveis sobre a
//   infraestrutura e a lógica interna da aplicação.
// ============================================================

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Caixa.Ans.Api.Middleware
{
    /// <summary>
    /// Middleware global de tratamento de exceções não capturadas.
    /// 
    /// Responsabilidades:
    /// 1. Captura qualquer exceção não tratada no pipeline HTTP
    /// 2. Registra o erro completo no log do servidor (ILogger)
    /// 3. Retorna ao cliente uma resposta padronizada (RFC 7807)
    /// 4. Protege contra vazamento de informações internas (OWASP)
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        // Delegate para o próximo middleware no pipeline (Chain of Responsibility).
        private readonly RequestDelegate _next;

        // Logger estruturado para registro de erros no servidor.
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Construtor com injeção do próximo middleware e do logger.
        /// O ASP.NET Core injeta automaticamente ambas as dependências.
        /// </summary>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Ponto de entrada do middleware. Envolve a execução do
        /// pipeline em um try-catch para captura global de exceções.
        /// </summary>
        /// <param name="context">Contexto HTTP da requisição atual.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Delega a execução para o próximo middleware no pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Registra o erro completo no log (stack trace inclusa)
                _logger.LogError(ex, "Erro não tratado capturado pelo middleware: {Message}", ex.Message);

                // Retorna resposta padronizada ao cliente (sem detalhes internos)
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Constrói e envia a resposta de erro padronizada (RFC 7807).
        /// 
        /// A mensagem ao cliente é genérica e profissional — os detalhes
        /// técnicos ficam restritos ao log do servidor, conforme as
        /// diretrizes OWASP de prevenção de Information Disclosure.
        /// </summary>
        /// <param name="context">Contexto HTTP para escrita da resposta.</param>
        /// <param name="exception">Exceção capturada (usada apenas para log).</param>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Define o content-type conforme RFC 7807
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Monta o objeto Problem Details com informações seguras
            var problemDetails = new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title = "Erro interno do servidor",
                status = context.Response.StatusCode,
                detail = "Ocorreu um erro inesperado. A equipe técnica foi notificada.",
                instance = context.Request.Path.Value
            };

            // Serializa e envia a resposta ao cliente
            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
