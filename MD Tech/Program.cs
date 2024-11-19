using MD_Tech;
using MD_Tech.Context;
using MD_Tech.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Logging
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");
builder.Logging.AddNLogWeb("nlog.config");
builder.WebHost.UseNLog();
builder.Services.AddScoped(typeof(LogsApi<>));

builder.WebHost.UseKestrel(options => options.ListenAnyIP(5294));

builder.Services.AddDbContext<MdtecnologiaContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o =>
{
    o.UseNodaTime();
    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
}));

builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(async o =>
{
    var rsa = System.Security.Cryptography.RSA.Create();
    rsa.ImportFromPem(await File.ReadAllTextAsync("public.pem"));
    var signKey = new RsaSecurityKey(rsa);

    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new()
    {
        ValidateAudience = false,
        IssuerSigningKey = signKey,
        ValidIssuer = "MDTech"
    };
});

builder.Services.TryAddTransient<IStorageApi, OciStorageApi>();
builder.Configuration.AddJsonFile("appsettings.json", false, true);

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    // Se añade la forma de autenticación en la Swagger para comodidad de hacer pruebas
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Autenticación JWT usando el esquema 'Bearer'. Ejemplo: 'Bearer {token}'"
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
    c.OperationFilter<AddResponseHeadersFilter>();
    // Configuración adicional para representar los tipos NodaTime en la documentación
    c.MapType<Instant>(() => new OpenApiSchema { Type = "string", Format = "date-time" });
    c.MapType<LocalDate>(() => new OpenApiSchema { Type = "string", Format = "date" });
    c.MapType<LocalTime>(() => new OpenApiSchema { Type = "string", Format = "time" });
    c.MapType<LocalDateTime>(() => new OpenApiSchema { Type = "string", Format = "date-time" });
}).ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb); 
    o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
