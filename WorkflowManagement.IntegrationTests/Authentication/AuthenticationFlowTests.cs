using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WorkflowManagement.Application.Users.Constants;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Infrastructure.Data;
using WorkflowManagement.IntegrationTests.Infrastructure;

namespace WorkflowManagement.IntegrationTests.Authentication
{
    public class AuthenticationFlowTests
        : IClassFixture<WorkflowManagementApiFactory>,
          IAsyncLifetime
    {
        private readonly WorkflowManagementApiFactory _factory;
        private readonly HttpClient _client;

        public AuthenticationFlowTests(
            WorkflowManagementApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            using var scope =
                _factory.Services.CreateScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<
                        WorkflowManagementDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            var roleManager =
                scope.ServiceProvider
                    .GetRequiredService<
                        RoleManager<IdentityRole<Guid>>>();

            await CreateRoleIfMissingAsync(
                roleManager,
                AppRoles.Admin);

            await CreateRoleIfMissingAsync(
                roleManager,
                AppRoles.Member);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        private static async Task CreateRoleIfMissingAsync(
            RoleManager<IdentityRole<Guid>> roleManager,
            string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var role = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName
            };

            var result =
                await roleManager.CreateAsync(role);

            Assert.True(
                result.Succeeded,
                string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description)));
        }

        [Fact]
        public async Task RegisterLoginAndAccessProtectedEndpoint_Succeeds()
        {
            // Arrange
            var email =
                $"integration-{Guid.NewGuid():N}@example.com";

            var password =
                "Integration123";

            var registration =
                new RegisterUserRequest
                {
                    DisplayName = "Integration User",
                    Email = email,
                    Password = password
                };

            // Act - Register
            var registerResponse =
                await _client.PostAsJsonAsync(
                    "/api/auth/register",
                    registration);

            // Assert - Registration
            Assert.Equal(
                HttpStatusCode.Created,
                registerResponse.StatusCode);

            // Act - Login
            var loginResponse =
                await _client.PostAsJsonAsync(
                    "/api/auth/login",
                    new LoginRequest
                    {
                        Email = email,
                        Password = password
                    });

            // Assert - Login
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResult>();

            Assert.NotNull(loginResult);
            Assert.True(loginResult.Succeeded);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    loginResult.Token));

            Assert.Contains(
                AppRoles.Member,
                loginResult.Roles);

            // Act - Authenticated request
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult.Token);

            var workItemsResponse =
                await _client.GetAsync(
                    "/api/work-items");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                workItemsResponse.StatusCode);
        }
    }
}