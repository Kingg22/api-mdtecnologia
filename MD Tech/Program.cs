using MD_Tech.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Logging
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");
builder.Logging.AddNLogWeb("nlog.config");
builder.WebHost.UseNLog();

builder.WebHost.UseKestrel(options => options.ListenAnyIP(5294));

builder.Services.AddDbContext<MdtecnologiaContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
