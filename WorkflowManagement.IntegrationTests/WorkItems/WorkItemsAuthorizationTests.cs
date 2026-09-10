using System.Net;
using WorkflowManagement.IntegrationTests.Infrastructure;

namespace WorkflowManagement.IntegrationTests.WorkItems
{
    public class WorkItemsAuthorizationTests
        : IClassFixture<WorkflowManagementApiFactory>
    {
        private readonly HttpClient _client;

        public WorkItemsAuthorizationTests(
            WorkflowManagementApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetWorkItems_WithoutToken_ReturnsUnauthorized()
        {
            // Act
            var response =
                await _client.GetAsync("/api/work-items");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }
    }
}