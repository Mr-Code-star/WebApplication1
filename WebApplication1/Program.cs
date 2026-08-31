using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using WebApplication1.Contexts.IAM.Application.Interfaces.OutboundServices;
using WebApplication1.Contexts.IAM.Application.Services;
using WebApplication1.Contexts.IAM.Domain.Repositories;
using WebApplication1.Contexts.IAM.Domain.Services;
using WebApplication1.Contexts.IAM.Infrastructure.Security;
using WebApplication1.iam.infrastructure.Email;
using WebApplication1.iam.infrastructure.Persitencia.MongoDb.Repositories;
using WebApplication1.iam.infrastructure.tokens;
using WebApplication1.iam.Interfaces.Facades;
using WebApplication1.Services;
using WebApplication1.shared.catalogs.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURAR SERIALIZADORES DE MONGODB
// ==========================================
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));

// ==========================================
// 2. CONFIGURAR MONGODB
// ==========================================
var mongoSettings = builder.Configuration.GetSection("MongoDB");
var mongoConnectionString = mongoSettings["ConnectionString"] 
                            ?? "mongodb://localhost:27017";
var mongoDatabaseName = mongoSettings["DatabaseName"] 
                        ?? "ferova";

builder.Services.AddScoped<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

// ==========================================
// 3. REGISTRAR SERVICIOS
// ==========================================

// 📌 Repositorios (Infrastructure)
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();

// 📌 Servicios de Infraestructura
builder.Services.AddScoped<IBcryptHashingService, BcryptHashingService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// 📌 Servicios de Email
builder.Services.AddHttpClient<IEmailService, ResendEmailService>();

// 📌 Servicios de Aplicación
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

// 📌 Facade
builder.Services.AddScoped<UserFacade>();

// 📌 Hosted Services
builder.Services.AddSingleton<DistrictRepository>();
builder.Services.AddHostedService<DatabaseSeeder>();

// ==========================================
// 4. CONFIGURAR CONTROLLERS
// ==========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ==========================================
// 5. CONFIGURAR SWAGGER
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ferova API",
        Version = "v1",
        Description = "Healthcare management API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingrese el token JWT: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Incluir comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// ==========================================
// 6. CONFIGURAR CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ==========================================
// 7. CONSTRUIR APLICACIÓN
// ==========================================
var app = builder.Build();

// ==========================================
// 8. CONFIGURAR MIDDLEWARES - IMPORTANTE: ORDEN
// ==========================================

// ✅ Swagger - DEBE IR ANTES de UseRouting
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ferova API V1");
        c.RoutePrefix = "api-docs"; // ✅ Esto hace que Swagger esté en /api-docs
        // c.RoutePrefix = string.Empty; // ✅ Si quieres que esté en la raíz
    });
}

// ✅ HTTPS (solo en producción)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// ✅ CORS
app.UseCors("AllowAll");

// ✅ Routing
app.UseRouting(); // ⚠️ IMPORTANTE: Debe estar antes de UseAuthentication/UseAuthorization

// ✅ Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// ✅ Mapear Controladores
app.MapControllers();

// ==========================================
// 9. INICIAR APLICACIÓN
// ==========================================
var port = builder.Configuration["PORT"] ?? "5002";
Console.WriteLine($"🚀 Aplicación iniciada en puerto: {port}");
Console.WriteLine($"📚 Swagger UI: http://localhost:{port}/api-docs");
Console.WriteLine($"📚 Swagger JSON: http://localhost:{port}/swagger/v1/swagger.json");
Console.WriteLine($"🔗 API Base: http://localhost:{port}/api/users");

app.Run();