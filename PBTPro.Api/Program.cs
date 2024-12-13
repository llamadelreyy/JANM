using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PBTPro.Api.Constants;
using PBTPro.Api.Controllers;
using PBTPro.Api.Services;
using PBTPro.DAL;
using PBTPro.Shared.Models.CommonService;
using Prometheus;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
IConfiguration configuration = builder.Configuration;

// Add services to the container.
// Register the configuration for IConfiguration
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
})
.AddJsonProtocol(options => {
    options.PayloadSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    //options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});

#region DB
builder.Services.AddDbContext<PBTProDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));
#endregion

#region Interface
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PBTPro Web API",
        Description = "PBTPro Web API, build: " + Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
    });
    swagger.DocumentFilter<SwaggerAPIPathPrefixInserter>();
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    //options.JsonSerializerOptions.MaxDepth = 64;
    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    //options.JsonSerializerOptions.PropertyNamingPolicy = null; // prevent camel case

    // Register the custom GeoJSON converter
    options.JsonSerializerOptions.Converters.Add(new GeometryJsonConverter());
});
#endregion

#region Utilities
//Email Sender
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

//Hosted Background Services Manager for long run Process
builder.Services.AddSingleton<PBTProBkgdSM>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<PBTProBkgdSM>());
#endregion

#region Security
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
    builder =>
    {
        builder.WithOrigins(
            "http://127.0.0.1",
            "https://127.0.0.1",
            "http://localhost",
            "https://localhost",
            "http://pbtpro.com.my",
            "https://pbtpro.com.my",
            "http://192.168.5.19",
            "https://192.168.5.19",
            "http://45.64.169.215",
            "https://45.64.169.215")
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials().AllowAnyHeader().AllowAnyMethod();
        //.SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<PBTProDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents();
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            if (context.Response.HasStarted)
            {
                return Task.CompletedTask; // Exit if the response has already started
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new ReturnViewModel
            {
                DateTime = DateTime.Now,
                ReturnCode = (int)HttpStatus.NoRequestAuthority,
                Status = "Unauthorized",
                Data = null,
                ReturnMessage = "Anda tidak dibenarkan untuk melihat kandungan ini!",
                ReturnParameter = null
            };

            var result = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(result).ContinueWith(t =>
            {
                // Mark the response as handled
                context.HandleResponse();
            });

        },
        OnMessageReceived = context =>
        {
            // Check if the request is for the SignalR hub
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/pushDataHub"))
            {
                // Skip token validation for SignalR hub
                context.NoResult();
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
        /*,
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/pushDataHub"))
            {
                string? accessToken = context.Request.Query["access_token"];
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                    {
                        accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    context.Token = accessToken;
                }
            }

            return Task.CompletedTask;
        }*/
    };
});

builder.Services.AddScoped<JWTTokenService>();
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();
//sseClientId remover for signalr server event protocol supports
app.UseMiddleware<MiddlewareRemoveSseClientId>();
app.UseMiddleware<MiddlewareErrorHandling>();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
//app.MapHub<PushDataHub>("/pushDataHub");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpMetrics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<PushDataHub>("/pushDataHub");
    endpoints.MapControllerRoute("default", "Web api");
});

app.Run();
