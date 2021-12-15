using System;
using System.Threading.Tasks;
using Common;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.Common.ApiLibrary.Middleware
{
    public class GlobalErrorHandlerMiddlewareWithStandardLogger
    {
        private readonly ILogger<GlobalErrorHandlerMiddlewareWithStandardLogger> _logger;
        private readonly CreateErrorResponse _createErrorResponse;
        private readonly RequestDelegate _next;

        public GlobalErrorHandlerMiddlewareWithStandardLogger(ILogger<GlobalErrorHandlerMiddlewareWithStandardLogger> logger, CreateErrorResponse createErrorResponse, RequestDelegate next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createErrorResponse = createErrorResponse ?? throw new ArgumentNullException(nameof(createErrorResponse));
            _next = next;
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await LogError(context, ex);
                await CreateErrorResponse(context, ex);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            var url = context.Request?.GetUri()?.AbsoluteUri;
            var body = await RequestUtils.GetRequestPartialBodyAsync(context);

            _logger.LogError(ex, $"There was an unhandled exception: url=[{url}], body=[{body.ToJson()}]");
        }

        private async Task CreateErrorResponse(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = 500;

            var response = _createErrorResponse(ex);
            var responseJson = JsonConvert.SerializeObject(response);

            await ctx.Response.WriteAsync(responseJson);
        }
    }
}