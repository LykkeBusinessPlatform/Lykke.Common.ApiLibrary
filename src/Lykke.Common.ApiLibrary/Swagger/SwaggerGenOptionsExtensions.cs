using System.IO;
using System.Reflection;
using Lykke.Common.ApiLibrary.Swagger.XmsEnum;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Application;

namespace Lykke.Common.ApiLibrary.Swagger
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Enables "x-ms-enum" swagger extension, wich allows Autorest tool generates enum or set of string constants for each server-side enum.
        /// </summary>
        /// <param name="swaggerOptions"></param>
        /// <param name="options">"x-ms-enum" extensions options. Default value is <see cref="XmsEnumExtensionsOptions.UseEnums"/></param>
        public static void EnableXmsEnumExtension(this SwaggerGenOptions swaggerOptions, XmsEnumExtensionsOptions options = XmsEnumExtensionsOptions.UseEnums)
        {
            swaggerOptions.SchemaFilter<XmsEnumSchemaFilter>(options);
            swaggerOptions.OperationFilter<XmsEnumOperationFilter>(options);
        }

        /// <summary>
        /// Includes source code's XML documentation into swagger document
        /// </summary>
        /// <remarks>
        /// Documentation will be included to swagger document only if assembly's 
        /// XML documentation file generation enabled and it's name corresponds to
        /// assembly name
        /// </remarks>
        public static void EnableXmlDocumentation(this SwaggerGenOptions swaggerOptions)
        {
            //Determine base path for the application.
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var entryAssembly = Assembly.GetEntryAssembly();

            //Set the comments path for the swagger json and ui.
            var xmlPath = Path.Combine(basePath, $"{entryAssembly.GetName().Name}.xml");

            if (File.Exists(xmlPath))
            {
                swaggerOptions.IncludeXmlComments(xmlPath);
            }
        }

        /// <summary>
        /// Setups swagger
        /// </summary>
        /// <param name="swaggerOptions"></param>
        /// <param name="apiVersion">Api version. e.g: "v1"</param>
        /// <param name="apiTitle">Api title</param>
        public static void DefaultLykkeConfiguration(this SwaggerGenOptions swaggerOptions, string apiVersion, string apiTitle)
        {
            swaggerOptions.SingleApiVersion(new Info
            {
                Version = apiVersion,
                Title = apiTitle
            });

            swaggerOptions.DescribeAllEnumsAsStrings();
            swaggerOptions.EnableXmsEnumExtension();
            swaggerOptions.EnableXmlDocumentation();
        }
    }
}