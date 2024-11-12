using MD_Tech.Contexts;
using MD_Tech.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Logging
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");
builder.Logging.AddNLogWeb("nlog.config");
builder.WebHost.UseNLog();

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

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen().ConfigureHttpJsonOptions(o => o.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

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

app.Run();
