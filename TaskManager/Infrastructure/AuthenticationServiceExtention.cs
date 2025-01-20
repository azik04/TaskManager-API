using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.DataProvider.Enums;

namespace TaskManager.Infrastructure;

public static class AuthenticationServiceExtention
{
    public static void AddAuthenticationConfiguration(this IServiceCollection services)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("0BD0E95C-6387-4135-A80E-489FF6E5C1DF")),
                 ValidateIssuer = false,
                 ValidateAudience = false,

            };
        });
        // Configure authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole(Role.Admin.ToString()));
            options.AddPolicy("User", policy => policy.RequireRole(Role.User.ToString(), Role.Admin.ToString()));
        });

    }

}
