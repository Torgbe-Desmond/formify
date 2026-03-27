using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FastTransfers.API.Extensions;

public static class ServiceExtensions
{
    // ── Swagger with JWT support ──────────────────────────────
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "FastTransfers API",
                Version = "v1",
                Description = "Document management API with schema-driven templates."
            });

            // Add Bearer token input to Swagger UI
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Enter your JWT token. Example: eyJhbGci..."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    // ── JWT Authentication ────────────────────────────────────
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
                                                           IConfiguration config)
    {
        var secret   = config["Jwt:Secret"]   ?? throw new InvalidOperationException("Jwt:Secret not set.");
        var issuer   = config["Jwt:Issuer"]   ?? "FastTransfers";
        var audience = config["Jwt:Audience"] ?? "FastTransfers";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = issuer,
                ValidAudience            = audience,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(secret))
            };
        });

        services.AddAuthorization();

        return services;
    }
}
