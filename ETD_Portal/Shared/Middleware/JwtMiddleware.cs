using ETD_Portal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Shared.Middleware
{
    // Shared/Middleware/JwtMiddleware.cs
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ETDPortalDbContext db)
        {
            // Read token from Authorization header
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, db, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context,
                                               ETDPortalDbContext db,
                                               string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(
                    context.RequestServices
                           .GetRequiredService<IConfiguration>()["Jwt:SecretKey"]);

                // Validate the token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = context.RequestServices
                                                      .GetRequiredService<IConfiguration>()["Jwt:Issuer"],
                    ValidAudience = context.RequestServices
                                                      .GetRequiredService<IConfiguration>()["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract claims from validated token
                var jwtToken = (JwtSecurityToken)validatedToken;
                var employeeId = int.Parse(
                    jwtToken.Claims.First(c => c.Type == "EmployeeId").Value);

                // Attach the full user object to HttpContext
                // so any controller can access it via HttpContext.Items["User"]
                context.Items["User"] = await db.Users
                    .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
            }
            catch
            {
                // Token validation failed — do nothing
                // Controller will return 401 via [Authorize] attribute
            }
        }
    }
}
