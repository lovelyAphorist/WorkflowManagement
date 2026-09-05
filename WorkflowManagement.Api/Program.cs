using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Application.WorkItems.Services;
using WorkflowManagement.Infrastructure.Data;
using WorkflowManagement.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework services
builder.Services.AddDbContext<WorkflowManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repository and service registrations
builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();

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
