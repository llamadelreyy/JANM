using PBTPro.DAL.Models.CommonServices;

namespace PBTPro.Api.Services
{
    public class MiddlewareErrorHandling
    {
        private readonly RequestDelegate _next;

        public MiddlewareErrorHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (!IsJsonResponse(context))
                {
                    await HandleExceptionAsync(context, ex);
                }
            }

            // Handle framework errors like 404, 413, etc. if not already handled
            if (context.Response.StatusCode >= 400)
            {
                if (!IsJsonResponse(context))
                {
                    await HandleErrorResponseAsync(context);
                }
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Handle application-specific errors (e.g., exceptions thrown in controllers or services)
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new ReturnViewModel
            {
                ReturnCode = StatusCodes.Status500InternalServerError,
                Status = "InternalServerError",
                ReturnMessage = "An unexpected error occurred."
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }

        private Task HandleErrorResponseAsync(HttpContext context)
        {
            var statusCode = context.Response.StatusCode;
            string status = "Error";
            string message = "An error occurred.";
            string source = "Framework"; // Default error source

            switch (statusCode)
            {
                case StatusCodes.Status400BadRequest:
                    status = "BadRequest";
                    message = "Bad Request.";
                    break;
                case StatusCodes.Status401Unauthorized:
                    status = "Unauthorized";
                    message = "Unauthorized.";
                    break;
                case StatusCodes.Status403Forbidden:
                    status = "Forbidden";
                    message = "Forbidden.";
                    break;
                case StatusCodes.Status404NotFound:
                    status = "NotFound";
                    message = "Not Found.";
                    break;
                case StatusCodes.Status413RequestEntityTooLarge:
                    status = "RequestEntityTooLarge";
                    message = "Request Entity Too Large.";
                    break;
                case StatusCodes.Status500InternalServerError:
                    status = "InternalServerError";
                    message = "Internal Server Error.";
                    break;
            }

            var errorResponse = new ReturnViewModel
            {
                ReturnCode = statusCode,
                Status = status,
                ReturnMessage = message
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }

        private bool IsJsonResponse(HttpContext context)
        {
            return context.Response.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
