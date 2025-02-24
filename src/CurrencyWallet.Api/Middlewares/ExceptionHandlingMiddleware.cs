#region Usings

using System.Net;
using System.Text.Json;
using CurrencyWallet.Domain.Exceptions;
using FluentValidation;
using Serilog;

#endregion

namespace CurrencyWallet.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        #region Private Methods

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            object errorResponse;
            response.StatusCode = exception switch
            {
                BadRequestException ex => (int)HttpStatusCode.BadRequest,
                NotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            if (exception is ValidationException validationException)
            {
                errorResponse = new
                {
                    message = "Validation failed",
                    errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                };
            }
            else
            {
                errorResponse = new { message = exception.Message };
            }

            return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        #endregion
    }
}
