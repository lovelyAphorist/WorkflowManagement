using Moq;
using WorkflowManagement.Application.Users.Services;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Application.WorkItems.Services;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Domain.Enums;
using Xunit;

namespace WorkflowManagement.UnitTests.WorkItems.Services
{
    public class WorkItemServiceTests
    {
        private readonly Mock<IWorkItemRepository> _repositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly WorkItemService _service;

        public WorkItemServiceTests()
        {
            _repositoryMock =
                new Mock<IWorkItemRepository>();

            _userServiceMock =
                new Mock<IUserService>();

            _service = new WorkItemService(
                _repositoryMock.Object,
                _userServiceMock.Object);
        }
        [Fact]
        public async Task CreateAsync_WithValidRequest_CreatesBacklogWorkItem()
        {
            // Arrange
            var request = new CreateWorkItemRequest
            {
                Title = "  Build automated test suite  ",
                Description = "Add unit tests.",
                Priority = WorkItemPriority.High,
                DueDate = new DateOnly(2026, 9, 18)
            };

            _repositoryMock
                .Setup(repository =>
                    repository.AddAsync(
                        It.IsAny<WorkItem>()))
                .ReturnsAsync(
                    (WorkItem workItem) => workItem);

            // Act
            var result =
                await _service.CreateAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);

            Assert.Equal(
                "Build automated test suite",
                result.Title);

            Assert.Equal(
                "Add unit tests.",
                result.Description);

            Assert.Equal(
                WorkItemStatus.Backlog,
                result.Status);

            Assert.Equal(
                WorkItemPriority.High,
                result.Priority);

            Assert.Equal(
                new DateOnly(2026, 9, 18),
                result.DueDate);

            Assert.NotEqual(
                default,
                result.CreatedAtUtc);

            Assert.Equal(
                result.CreatedAtUtc,
                result.UpdatedAtUtc);

            _repositoryMock.Verify(
                repository =>
                    repository.AddAsync(
                        It.Is<WorkItem>(workItem =>
                            workItem.Title ==
                                "Build automated test suite" &&
                            workItem.Status ==
                                WorkItemStatus.Backlog)),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenWorkItemDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(id))
                .ReturnsAsync((WorkItem?)null);

            // Act
            var result =
                await _service.DeleteAsync(id);

            // Assert
            Assert.False(result);

            _repositoryMock.Verify(
                repository =>
                    repository.DeleteAsync(
                        It.IsAny<WorkItem>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenWorkItemExists_DeletesAndReturnsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var workItem = new WorkItem
            {
                Id = id
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(id))
                .ReturnsAsync(workItem);

            // Act
            var result =
                await _service.DeleteAsync(id);

            // Assert
            Assert.True(result);

            _repositoryMock.Verify(
                repository =>
                    repository.DeleteAsync(workItem),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenPriorityChanges_CreatesPriorityHistoryEntry()
        {
            // Arrange
            var id = Guid.NewGuid();

            var workItem = new WorkItem
            {
                Id = id,
                Title = "Test work item",
                Description = "Description",
                Status = WorkItemStatus.Backlog,
                Priority = WorkItemPriority.Medium,
                DueDate = new DateOnly(2026, 9, 20),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            var request = new UpdateWorkItemRequest
            {
                Title = workItem.Title,
                Description = workItem.Description,
                Status = workItem.Status,
                Priority = WorkItemPriority.High,
                DueDate = workItem.DueDate
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(id))
                .ReturnsAsync(workItem);

            _repositoryMock
                .Setup(repository =>
                    repository.UpdateAsync(
                        workItem,
                        It.IsAny<IReadOnlyCollection<WorkItemHistory>>()))
                .ReturnsAsync(workItem);

            // Act
            var result =
                await _service.UpdateAsync(id, request);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                WorkItemPriority.High,
                result.Priority);

            _repositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        workItem,
                        It.Is<IReadOnlyCollection<WorkItemHistory>>(
                            history =>
                                history.Count == 1 &&
                                history.Single().ChangeType ==
                                    WorkItemChangeType.Priority &&
                                history.Single().OldValue ==
                                    WorkItemPriority.Medium.ToString() &&
                                history.Single().NewValue ==
                                    WorkItemPriority.High.ToString()
                        )),
                Times.Once);
        }
    }
}