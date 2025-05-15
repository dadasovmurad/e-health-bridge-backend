using System.Text.Json;

namespace EHealthBridgeAPI.API.Extensions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // davam et
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                var statusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

                var response = new
                {
                    StatusCode = statusCode,
                    Message = _env.IsDevelopment() ? ex.Message : GetGenericMessage(statusCode),
                    Details = _env.IsDevelopment() ? ex.StackTrace : null
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
            }
        }

        private string GetGenericMessage(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status404NotFound => "Resurs tapılmadı.",
                StatusCodes.Status401Unauthorized => "Giriş icazəsi yoxdur.",
                _ => "Xəta baş verdi."
            };
        }

        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }
    }
}
