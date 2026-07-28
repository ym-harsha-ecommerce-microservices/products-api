using System.Reflection;
using eCommerce.API.EndPoints;
using eCommerce.API.Middlewares;
using eCommerce.BLL;
using eCommerce.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);

builder.Services.AddBusinessLogicLayer();

builder.Services.ConfigureHttpJsonOptions(configOptions =>
{
    configOptions.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(config =>
    {
        config.WithOrigins("http://localhost:4200")
            .AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "eCommerce Products API",
        Version = "v1",
        Description = "API for managing products in the eCommerce platform."
    });

    // XML comments from the API project (endpoint summaries via WithSummary/WithDescription
    // don't need this, but controllers, if any, and shared types do)
    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
    if (File.Exists(apiXmlPath))
        options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);

    // XML comments from the BLL project, so DTO property comments show in Swagger
    var bllXmlFile = "eCommerce.BLL.xml";
    var bllXmlPath = Path.Combine(AppContext.BaseDirectory, bllXmlFile);
    if (File.Exists(bllXmlPath))
        options.IncludeXmlComments(bllXmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlingMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();


app.MapProductAPIEndpoints();


app.Run();