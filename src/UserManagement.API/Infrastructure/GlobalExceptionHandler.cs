using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ocorreu uma exceção não tratada durante a requisição: {Message}", exception.Message);

        //Cria uma resposta estruturada (ProblemDetails)
        var problemDetails = new ProblemDetails
        {
            Title = "Ocorreu um erro.",
        };
        
        switch (exception)
        {
            case ArgumentException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Pedido Inválido (Bad Request)";
                break;
            case InvalidOperationException:
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Conflito de Estado (Conflict)";
                break;
            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Erro Interno do Servidor";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retorna true pra sinalizar que a exceção foi tratada com sucesso
        return true;
    }
}