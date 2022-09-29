using System;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lykke.Common.ApiLibrary.Middleware
{
    [PublicAPI]
    public static class MiddlewareApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure application to use standart Lykke middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="componentName">Component name for logs</param>
        /// <param name="createGlobalErrorResponse">Create global error response delegate</param>
        /// <param name="logClientErrors">log 4xx errors</param>
        /// <param name="useStandardLogger">Use Microsoft ILogger interface for global error handling</param>
        [Obsolete]
        public static void UseLykkeMiddleware(
            this IApplicationBuilder app, 
            string componentName, 
            CreateErrorResponse createGlobalErrorResponse,
            bool logClientErrors = false,
            bool useStandardLogger = false)
        {
            app.Use(async (context, next) =>
            {
                // enable ability to seek on request stream within any host,
                // but not only Kestrel, for any subsequent middleware
                context.Request.EnableRewind();
                await next();
            });
            
            if (useStandardLogger)
            {
                var log = app.ApplicationServices.GetRequiredService<ILogger<GlobalErrorHandlerMiddlewareWithStandardLogger>>();
                
                app.UseMiddleware<GlobalErrorHandlerMiddlewareWithStandardLogger>(
                    log,
                    createGlobalErrorResponse);    
            }
            else
            {
                var log = app.ApplicationServices.GetRequiredService<ILog>();
                
                app.UseMiddleware<GlobalErrorHandlerMiddleware>(
                    log,
                    componentName, 
                    createGlobalErrorResponse);   
            }

            if (logClientErrors)
            {
                var log = app.ApplicationServices.GetRequiredService<ILog>();
                
                app.UseMiddleware<ClientErrorHandlerMiddleware>(
                    log,
                    componentName);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="componentName"></param>
        /// <param name="createGlobalErrorResponse"></param>
        /// <param name="logClientErrors"></param>
        public static void UseLykkeMiddleware(
            this IApplicationBuilder app, 
            string componentName, 
            CreateErrorResponse createGlobalErrorResponse,
            bool logClientErrors = false)
        {
            app.Use(async (context, next) =>
            {
                // enable ability to seek on request stream within any host,
                // but not only Kestrel, for any subsequent middleware
                context.Request.EnableRewind();
                await next();
            });
            
            if (logClientErrors)
            {
                var log = app.ApplicationServices.GetRequiredService<ILog>();
                
                app.UseMiddleware<ClientErrorHandlerMiddleware>(
                    log,
                    componentName);
            }
        }

        /// <summary>
        /// Configure application to use standard Lykke middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="createGlobalErrorResponse">Create global error response delegate</param>
        /// <param name="logClientErrors">Enables logging of the requests with 4xx response codes</param>
        /// <param name="useStandardLogger">Use Microsoft ILogger interface for global error handling</param>
        public static void UseLykkeMiddleware(
            [NotNull] this IApplicationBuilder app, 
            [NotNull] CreateErrorResponse createGlobalErrorResponse,
            bool logClientErrors = false,
            bool useStandardLogger = false)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (createGlobalErrorResponse == null)
            {
                throw new ArgumentNullException(nameof(createGlobalErrorResponse));
            }

            app.Use(async (context, next) =>
            {
                // enable ability to seek on request stream within any host,
                // but not only Kestrel, for any subsequent middleware
                context.Request.EnableRewind();
                await next();
            });

            var logFactory = app.ApplicationServices.GetRequiredService<ILogFactory>();

            if (useStandardLogger)
            {
                var log = app.ApplicationServices.GetRequiredService<ILogger<GlobalErrorHandlerMiddlewareWithStandardLogger>>();
                
                app.UseMiddleware<GlobalErrorHandlerMiddlewareWithStandardLogger>(
                    log,
                    createGlobalErrorResponse);
            }
            else
            {
                app.UseMiddleware<GlobalErrorHandlerMiddleware>(
                    logFactory,
                    createGlobalErrorResponse);
            }

            if (logClientErrors)
            {
                app.UseMiddleware<ClientErrorHandlerMiddleware>(logFactory);
            }
        }

        /// <summary>
        /// Configure application to use forwarded headers
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseLykkeForwardedHeaders(this IApplicationBuilder app)
        {
            var forwardingOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            };

            forwardingOptions.KnownNetworks.Clear(); //its loopback by default
            forwardingOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardingOptions);
        }
    }
}
