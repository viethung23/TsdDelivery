using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TsdDelivery.Api.Filters;
using TsdDelivery.Api.Middlewares;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Services.V1;

namespace TsdDelivery.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services,JwtSettings jwtSettings)
    {
        services.AddControllers(config =>
        {
            config.Filters.Add(typeof(TsdDeliveryExceptionHandler));
        });
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(options => 
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true; 
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        //------------------------------------------------------------------------------------
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareHandler>();
        //----------------------------------------------------------------------------------------------
        //for appear summary
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TsdDelivery API",
                Description = "An ASP.NET Core Web API for EXE02 Project",
                //TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "VietHung",
                    Url = new Uri("https://example.com/contact")
                }
            });
            
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = "TsdDelivery API",
                Description = "An ASP.NET Core Web API for EXE02 Project",
                //TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "VietHung",
                    Url = new Uri("https://example.com/contact")
                }
            });
            
            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer {Generated-JWT-Token}`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
            
            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        
        //----------------------------------------------------------------------
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(jwtSettings.ExpiryMinutes) 
                };
            });
        //------------------------------------------------------------------------
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("RequireDriverRole", policy => policy.RequireRole("DRIVER"));
            opt.AddPolicy("RequireUserRole", policy => policy.RequireRole("USER"));
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("ADMIN"));
            opt.AddPolicy("AdminOrUser", policy => policy.RequireRole("ADMIN", "USER"));
        });
        
        //------------------------------------------------------------------------
        
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddHttpContextAccessor();
        services.AddSingleton<GlobalExceptionMiddleware>();

        //services.AddAutoMapper(typeof(UserMapConfig).Assembly);
        return services;
    }
}
