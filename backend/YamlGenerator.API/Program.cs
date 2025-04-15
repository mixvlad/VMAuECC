using Microsoft.OpenApi.Models;
using System.Reflection;
using YamlGenerator.Core.Services;

var builder = WebApplication.CreateBuilder(args);
string templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Templates");



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<LocalizationService>();
builder.Services.AddSingleton<ControlTypeService>();
builder.Services.AddSingleton(new YamlGeneratorService());


// Улучшенная настройка Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "YAML Generator API",
        Description = "API для генерации YAML-файлов конфигурации сбора данных",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@example.com"
        }
    });

    // Включаем XML-комментарии для Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddSingleton<YamlGeneratorService>();
builder.Services.AddSingleton<ControlTypeService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Включаем Swagger и в Production, и в Development
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "YAML Generator API v1");
    options.RoutePrefix = string.Empty; // Swagger будет доступен по корневому URL
});

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Удаляем или комментируем код WeatherForecast, так как он не нужен
// var summaries = new[] { ... };
// app.MapGet("/weatherforecast", () => { ... });

app.Run();
