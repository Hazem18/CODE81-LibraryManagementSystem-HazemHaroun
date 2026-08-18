using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Registered so it can be resolved from the same DI scope as the
        // DbContext being built below (AddInterceptors needs an instance,
        // and AuditSaveChangesInterceptor itself depends on the scoped
        // ICurrentUserService).
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Application code depends on IApplicationDbContext, not the concrete
        // EF class - keeps handlers unaware they're talking to SQL Server.
        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ManageSystemUsers", policy => policy.RequireRole(Domain.Constants.Roles.Administrator));

            options.AddPolicy("ManageBooks", policy => policy.RequireRole(Domain.Constants.Roles.Administrator, Domain.Constants.Roles.Librarian));
            options.AddPolicy("ManageMembers", policy => policy.RequireRole(Domain.Constants.Roles.Administrator, Domain.Constants.Roles.Librarian));

            options.AddPolicy("ViewCatalog", policy => policy.RequireRole(Domain.Constants.Roles.Administrator, Domain.Constants.Roles.Librarian, Domain.Constants.Roles.Staff));
            options.AddPolicy("ProcessBorrowReturn", policy => policy.RequireRole(Domain.Constants.Roles.Administrator, Domain.Constants.Roles.Librarian, Domain.Constants.Roles.Staff));

            options.AddPolicy("ViewActivityLogs", policy => policy.RequireRole(Domain.Constants.Roles.Administrator, Domain.Constants.Roles.Librarian));
        });

        return services;
    }
}
