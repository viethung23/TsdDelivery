using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using TsdDelivery.Api;
using TsdDelivery.Api.Middlewares;
using TsdDelivery.Application.Commons;
using TsdDelivery.Infrastructures;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUrl = builder.Configuration.GetSection("KeyVault:KeyVaultUrl");
var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId");
var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
var keyVaultDirectoryId = builder.Configuration.GetSection("KeyVault:DirectoryId");

var credential = new ClientSecretCredential(keyVaultDirectoryId.Value!, keyVaultClientId.Value!, keyVaultClientSecret.Value!);
builder.Configuration.AddAzureKeyVault(keyVaultUrl.Value!, keyVaultClientId.Value!, keyVaultClientSecret.Value!, new DefaultKeyVaultSecretManager());

// if want to config more 
//var client = new SecretClient(new Uri(keyVaultUrl.Value!.ToString()), credential);

var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructuresService(configuration.DatabaseConnection,configuration.RedisConnection);
builder.Services.AddWebAPIService(configuration.JwtSettings);
builder.Services.AddSingleton(configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000",
                "http://localhost:3001",
                "http://localhost:8080",
                "http://localhost:8000",
                "http://143.198.196.235:8000",
                "https://exe202.vercel.app",
                "https://www.tsdproject.online",
                "https://tsdproject.online")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
    });
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
});

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "My Website",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter{
            User = "admin",
            Pass = "123456"
        }
    }
});

app.Run();
public partial class Program { }
