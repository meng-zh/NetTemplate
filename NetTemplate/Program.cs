var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
#if DEBUG
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<NetTemplate.Filters.SwaggerHeaderFilter>();
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "NetTemplate",
        Description = @""
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
#endif
#region ÈÕÖ¾ÅäÖÃ

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    config.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
    config.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
});

#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
#if DEBUG
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Shown api";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NetTemplateApi");
    });
}
#endif

app.UseAuthorization();

app.MapControllers();

app.Run();
