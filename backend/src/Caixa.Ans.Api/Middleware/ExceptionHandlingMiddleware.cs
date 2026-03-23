// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Middleware: Filtro que intercepta todas as requisicoes antes de chegar ao Controller.
// - Exception: Erro inesperado que ocorre durante o processamento.
// - Problem Details (RFC 7807): Formato padrao para retornar erros em APIs REST.
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
    /// Middleware global que captura excecoes nao tratadas,
    /// registra no log e retorna uma resposta padronizada ao cliente.
    /// Garante que a stack trace nunca seja exposta ao front-end.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro nao tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problemDetails = new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title = "Erro interno do servidor",
                status = context.Response.StatusCode,
                detail = "Ocorreu um erro inesperado. A equipe tecnica foi notificada.",
                instance = context.Request.Path.Value
            };

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
