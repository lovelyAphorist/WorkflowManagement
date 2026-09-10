using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using WorkflowManagement.Infrastructure.Data;

namespace WorkflowManagement.IntegrationTests.Infrastructure
{
    public class WorkflowManagementApiFactory
        : WebApplicationFactory<Program>
    {
        public WorkflowManagementApiFactory()
        {
            Environment.SetEnvironmentVariable(
                "Jwt__Key",
                "integration-test-signing-key-that-is-long-enough-12345");

            Environment.SetEnvironmentVariable(
                "Jwt__Issuer",
                "WorkflowManagement.Api.Tests");

            Environment.SetEnvironmentVariable(
                "Jwt__Audience",
                "WorkflowManagement.Web.Tests");

            Environment.SetEnvironmentVariable(
                "Jwt__ExpirationMinutes",
                "60");
        }

        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor =
                    services.SingleOrDefault(
                        service =>
                            service.ServiceType ==
                            typeof(
                                IDbContextOptionsConfiguration<
                                    WorkflowManagementDbContext>));

                if (dbContextDescriptor is not null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbConnectionDescriptor =
                    services.SingleOrDefault(
                        service =>
                            service.ServiceType ==
                            typeof(DbConnection));

                if (dbConnectionDescriptor is not null)
                {
                    services.Remove(dbConnectionDescriptor);
                }

                services.AddSingleton<DbConnection>(_ =>
                {
                    var connection =
                        new SqliteConnection(
                            "DataSource=:memory:");

                    connection.Open();

                    return connection;
                });

                services.AddDbContext<
                    WorkflowManagementDbContext>(
                    (serviceProvider, options) =>
                    {
                        var connection =
                            serviceProvider
                                .GetRequiredService<DbConnection>();

                        options.UseSqlite(connection);
                    });
            });
        }

        protected override void Dispose(bool disposing)
        {
            Environment.SetEnvironmentVariable(
                "Jwt__Key",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__Issuer",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__Audience",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__ExpirationMinutes",
                null);

            base.Dispose(disposing);
        }
    }
}