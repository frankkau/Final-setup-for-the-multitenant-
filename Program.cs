using System.Text;
using CouncilRevenueCollection;
using CouncilRevenueCollection.Application;
using CouncilRevenueCollection.Application.Identity;
using CouncilRevenueCollection.Application.Implementation;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Application.Service;
using CouncilRevenueCollection.Models.Entity;
using CouncilRevenueCollection.Seeding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using AuthenticationService = CouncilRevenueCollection.Application.Implementation.AuthenticationService;
using IAuthenticationService = CouncilRevenueCollection.Application.InterfaceClass.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// HttpContext
builder.Services.AddHttpContextAccessor();

#region Identity


builder.Services
    .AddIdentity<User, ApplicationRole>(options =>
    {
        // Password
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // User
        options.User.RequireUniqueEmail = true;

        // Lockout
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleValidator<TenantRoleValidator>()
    .AddDefaultTokenProviders();

#endregion

#region Dependency Injection

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();


#endregion

#region JWT

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var tenantService =
                    context.HttpContext.RequestServices
                        .GetRequiredService<ITenantService>();

                var currentTenant = tenantService.GetCurrentTenantId();

                var tokenTenant =
                    context.Principal?
                        .FindFirst("TenantId")?
                        .Value;

                if (string.IsNullOrWhiteSpace(currentTenant) ||
                    string.IsNullOrWhiteSpace(tokenTenant) ||
                    !string.Equals(currentTenant, tokenTenant,
                        StringComparison.OrdinalIgnoreCase))
                {
                    context.Fail("Token does not belong to the current tenant.");
                }

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

#endregion

#region Authorization

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Permissions.Revenue.Collect", policy =>
    {
        policy.RequireClaim("permission", "Revenue.Collect");
    });

#endregion

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsJsonAsync(new
        {
            message = ex?.Message ?? "An unexpected error occurred."
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Council Revenue Collection API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient);
    });

    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .ExcludeFromDescription();
}

app.UseRouting();

// Resolve tenant before authentication
app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed database
await DatabaseSeeder.SeedAsync(app.Services);

app.Run();