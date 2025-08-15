using System.Net;
using System.Text.Json;
using SprintQuiz.Api.Exceptions;

namespace SprintQuiz.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Une erreur inattendue s'est produite");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse();

            switch (exception)
            {
                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = argEx.Message;
                    response.Details = "Paramètre invalide";
                    break;

                case KeyNotFoundException keyEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = keyEx.Message;
                    response.Details = "Ressource non trouvée";
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Accès non autorisé";
                    response.Details = "Vous n'avez pas les permissions nécessaires";
                    break;

                case InvalidOperationException invOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = invOpEx.Message;
                    response.Details = "Opération invalide";
                    break;

                case ConcurrencyException concurrencyEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Message = concurrencyEx.Message;
                    response.Details = concurrencyEx.EntityExists
                        ? "La ressource a été modifiée par un autre utilisateur. Veuillez recharger et réessayer."
                        : "La ressource a été supprimée par un autre utilisateur.";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Une erreur interne du serveur s'est produite";
                    response.Details = "Veuillez réessayer plus tard ou contacter le support";
                    break;
            }

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}