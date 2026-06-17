using AutoMapper;
using ETD_Portal.Auth_Module.DAL.Classes;
using ETD_Portal.Auth_Module.DAL.Interfaces;
using ETD_Portal.Auth_Module.Service.Classes;
using ETD_Portal.Auth_Module.Service.Interfaces;
using ETD_Portal.Data;
using ETD_Portal.HR_Management.BLL.Classes;
using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Classes;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.BLL.Classes;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DAL.Classes;
using ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.Mappings;
using ETD_Portal.Reservation_Mgmt.BLL.Classes;
using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DAL.Classes;
using ETD_Portal.Reservation_Mgmt.DAL.Interfaces;
using ETD_Portal.Reservation_Mgmt.Mappings;
using ETD_Portal.Shared.Helpers;
using ETD_Portal.Shared.Middleware;
using ETD_Portal.TravelPlanner.BLL.Classes;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DAL.Classes;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using System.Text;



internal class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting ETD_Portal");

            var builder = WebApplication.CreateBuilder(args);

            // Replace default logging with Serilog, reading from appsettings.json
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddDbContext<ETDPortalDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ETDPortalDb")));

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ReservationMappingProfile>();
                cfg.AddProfile<ReimbursementMappingProfile>();
            });

            builder.Services.AddScoped<IGradeRepo, GradeRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IGradeHistoryRepo, GradeHistoryRepo>();

            builder.Services.AddScoped<IGradeServices, GradeServices>();
            builder.Services.AddScoped<IUserServices, UserServices>();

            builder.Services.AddScoped<ILocationRepo, LocationRepo>();
            builder.Services.AddScoped<ITravelRequestRepo, TravelRequestRepo>();
            builder.Services.AddScoped<ITravelBudgetRepo, TravelBudgetRepo>();

            builder.Services.AddScoped<ILocationServices, LocationServices>();
            builder.Services.AddScoped<ITravelRequestServices, TravelRequestServices>();
            builder.Services.AddScoped<IBudgetAllocationServices, BudgetAllocationServices>();

            builder.Services.AddScoped<IReservationTypeRepo, ReservationTypeRepo>();
            builder.Services.AddScoped<IReservationRepo, ReservationRepo>();
            builder.Services.AddScoped<IReservationDocRepo, ReservationDocRepo>();

            builder.Services.AddScoped<IReservationTypeServices, ReservationTypeServices>();
            builder.Services.AddScoped<IReservationServices, ReservationServices>();
            builder.Services.AddScoped<IReservationDocServices, ReservationDocServices>();

            builder.Services.AddScoped<IReimbursementTypeRepo, ReimbursementTypeRepo>();
            builder.Services.AddScoped<IReimbursementTypeServices, ReimbursementTypeServices>();

            builder.Services.AddScoped<IReimbursementRepo, ReimbursementRepo>();
            builder.Services.AddScoped<IReimbursementServices, ReimbursementServices>();

            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSingleton<JwtHelper>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Travel Desk Portal API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT access token below.\n\nExample: eyJhbGciOiJIUzI1NiIs..."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000") // React app URL
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            var app = builder.Build();


            app.Use(async (context, next) =>
            {
                var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                                    ?? Guid.NewGuid().ToString("N")[..12];

                context.Response.Headers["X-Correlation-Id"] = correlationId;

                using (LogContext.PushProperty("CorrelationId", correlationId))
                {
                    await next();
                }
            });

            // 2. Serilog request logging — one summary line per HTTP request
            //    (replaces the noisy default ASP.NET request logging you just suppressed)
            app.UseSerilogRequestLogging(opts =>
            {
                opts.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<JwtMiddleware>();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "ETD_Portal terminated unexpectedly during startup");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}