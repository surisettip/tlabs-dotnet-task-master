using TestApp.ToDoList.Store;
using TestApp.ToDoList.Module;
using TestApp.ToDoList.Tracker;
using TestApp.ToDoList.Repository;
using TestApp.Server.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TestApp.Server.Auth;
using System;
using Microsoft.OpenApi.Models;   
namespace TestApp.Server
{
  public class Startup
  {
    IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
      this.configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
      // Add DB
      services.AddDbContext<ToDoListDbContext>();

      // Add controllers
      services.AddControllers();

      // Configure app services
      services.AddScoped<IToDoListTracker, ToDoListTracker>();
      // Changed to scoped
      services.AddScoped<IToDoItemsRepository, ToDoItemsRepository>();
      services.AddScoped<ToDoListEntityModel>();
   // Bonus: JWT authentication
      services.AddSingleton<IJwtTokenService, JwtTokenService>();

      var jwtSection = configuration.GetSection("Jwt");
      var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");

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
          ValidIssuer = jwtSection["Issuer"],
          ValidAudience = jwtSection["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
          ClockSkew = TimeSpan.Zero
        };
      });

      services.AddAuthorization();
      services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      // Added AllowAnyMethod() for complete CORS support.
                      .AllowAnyMethod();
                });
      });

      services.AddEndpointsApiExplorer();
       services.AddSwaggerGen(options =>
      {
        // Bonus: let Swagger UI send a Bearer token so /api/tasks can be tested.
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = SecuritySchemeType.Http,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "Enter the token returned by POST /api/auth/login (without the word 'Bearer')."
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
          }
        });
      });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svcProv)
    {

      app.UseMiddleware<ExceptionHandlingMiddleware>(); 
      // Enable Swagger in all environments
      app.UseSwagger();
      app.UseSwaggerUI();

      var appLifetime = svcProv.GetRequiredService<IHostApplicationLifetime>();
      appLifetime.ApplicationStarted.Register(onApplicationStarted);

      app.UseRouting();
   
      app.UseCors();  
      app.UseHttpsRedirection(); 
        
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
        }
      );
    }

    void onApplicationStarted()
    {
      // Do nothing
    }

  }
}