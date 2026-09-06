using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.Users.Services;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Application.WorkItems.Services;
using WorkflowManagement.Infrastructure.Data;
using WorkflowManagement.Infrastructure.Identity;
using WorkflowManagement.Infrastructure.Repositories;

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
builder.Services.AddSwaggerGen();

// Database.
builder.Services.AddDbContext<WorkflowManagementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();