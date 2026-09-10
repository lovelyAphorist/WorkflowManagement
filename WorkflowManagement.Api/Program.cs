using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.Users.Services;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Application.WorkItems.Services;
using WorkflowManagement.Infrastructure.Data;
using WorkflowManagement.Infrastructure.Identity;
using WorkflowManagement.Infrastructure.Repositories;
using WorkflowManagement.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and configure JSON serialization.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });

// Swagger / OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Database.
builder.Services.AddDbContext<WorkflowManagementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        }));

// Required by ASP.NET Identity token providers.
builder.Services.AddDataProtection();

// ASP.NET Core Identity.
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<WorkflowManagementDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Application services.
builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddScoped<IWorkItemCommentRepository,WorkItemCommentRepository>();
builder.Services.AddScoped<IWorkItemCommentService, WorkItemCommentService>();

var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException(
        "JWT signing key is missing.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
     new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,

         ValidIssuer = jwtOptions.Issuer,
         ValidAudience = jwtOptions.Audience,

         IssuerSigningKey =
             new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(jwtOptions.Key)),

         RoleClaimType = ClaimTypes.Role,

         ClockSkew = TimeSpan.Zero
     };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevelopment", policy =>
    {
        policy
            .WithOrigins("http://localhost:60727")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<WorkflowManagementDbContext>();

    await dbContext.Database.MigrateAsync();

    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole<Guid>>>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<
                UserManager<ApplicationUser>>();

    await IdentitySeeder.SeedRolesAsync(
        roleManager,
        userManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactDevelopment");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}