using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    public class XmsEnumOperationFilter : IOperationFilter
    {
        private readonly XmsEnumExtensionsOptions _options;

        public XmsEnumOperationFilter(XmsEnumExtensionsOptions options)
        {
            _options = options;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            var invalidParameters = context.ApiDescription.ParameterDescriptions.Where(p => p.Type == null).ToArray();

            if (invalidParameters.Any())
            {
                var invalidParameter = invalidParameters.First();
                var message =
$@"Invalid parameter found. Probably parameter source mismatched. (e.g. You specified [FromQuery] for the path parameter).

Action: {context.ApiDescription?.ActionDescriptor.DisplayName}
Relaive path: {context.ApiDescription?.RelativePath}
OperationId: {operation.OperationId}
Parameter: {invalidParameter?.Name}
Expected parameter source: {invalidParameter?.Source.DisplayName}";
                throw new InvalidOperationException(message);
            }
            
            foreach (var parameter in GetEnumPerameters(context))
            {
                var operationParameter = operation.Parameters.Single(p => p.Name == parameter.Name);

                XmsEnumExtensionApplicator.Apply(operationParameter.Extensions, parameter.Type, _options);
            }
        }

        private static IEnumerable<(Type Type, string Name)> GetEnumPerameters(OperationFilterContext context)
        {
            return context
                .ApiDescription
                .ParameterDescriptions
                .Select(p => (Type: TryGetEnumType(p), Name: p.Name))
                .Where(x => x.Type != null);
        }

        private static Type TryGetEnumType(ApiParameterDescription parameter)
        {
            if (parameter.Type.GetTypeInfo().IsEnum)
            {
                return parameter.Type;
            }

            var nullableUnderlyingType = Nullable.GetUnderlyingType(parameter.Type);

            return nullableUnderlyingType?.GetTypeInfo().IsEnum == true
                ? nullableUnderlyingType 
                : null;
        }
    }
}