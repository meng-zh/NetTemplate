using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetTemplate.Filters
{
    /// <summary>
    /// 在swagger页面上增加header的输入界面
    /// </summary>
    public class SwaggerHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context != null && context.ApiDescription != null && context.ApiDescription.RelativePath != null
            && context.ApiDescription.RelativePath.StartsWith("api/xxx"))
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Temper",
                    In = ParameterLocation.Header,
                    Description = ""
                });
            }
        }
    }
}
