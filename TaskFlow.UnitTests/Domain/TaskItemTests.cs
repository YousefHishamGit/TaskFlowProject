using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.UnitTests.Domain
{
    public class TaskItemTests
    {
        private static Guid GetValidId() => Guid.NewGuid();

        private static TaskItem CreateDefaultTaskItem(
            string title = "Fix bug",
            string description = "desc",
            int priority = 2,
            DateTime? dueDate = null,
            Guid? projectId = null,
            Guid? createdById = null)
        {
            return new TaskItem(
                title,
                description,
                priority,
                dueDate,
                projectId ?? GetValidId(),
                createdById ?? GetValidId()
            );
        }
        private static TaskItem CreateTaskInStatus(TaskItemStatus targetStatus)
        {
            var task = CreateDefaultTaskItem();
            if (targetStatus == TaskItemStatus.InProgress)
            {
                task.ChangeStatus(TaskItemStatus.InProgress);
            }
            else if (targetStatus == TaskItemStatus.Done)
            {
                task.ChangeStatus(TaskItemStatus.InProgress);
                task.ChangeStatus(TaskItemStatus.Done);
            }
            return task;
        }




        [Fact]
        public void Constructor_WithValidData_CreatesTaskSuccessfully()
        {
            // Arrange & Act
            var dueDate = DateTime.UtcNow.AddDays(5);
            var projectId = GetValidId();
            var createdById = GetValidId();

            var task = new TaskItem("  Fix bug  ", "  desc  ", 2, dueDate, projectId, createdById);

            // Assert
            Assert.Equal("Fix bug", task.Title);
            Assert.Equal("desc", task.Description);
            Assert.Equal(TaskItemStatus.Todo, task.Status);
            Assert.Equal(2, task.Priority);
            Assert.Equal(dueDate, task.DueDate);
            Assert.Equal(projectId, task.ProjectId);
            Assert.Equal(createdById, task.CreatedById);
            Assert.Null(task.AssigneeId);
            Assert.Null(task.CompletedAt);
            Assert.NotEqual(Guid.Empty, task.Id);
        }

        [Fact]
        public void Constructor_WithNullDescription_StoresEmptyString()
        {
            // Arrange & Act
            var task = new TaskItem("Fix bug", null!, 2, null, GetValidId(), GetValidId());

            // Assert
            Assert.NotNull(task.Description);
            Assert.Equal(string.Empty, task.Description);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ab")]
        public void Constructor_WithInvalidTitle_ThrowsDomainException(string invalidTitle)
        {
            // Arrange, Act & Assert
            Assert.Throws<DomainException>(() => CreateDefaultTaskItem(title: invalidTitle));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(-1)]
        public void Constructor_WithInvalidPriority_ThrowsDomainException(int invalidPriority)
        {
            // Arrange, Act & Assert
            Assert.Throws<DomainException>(() => CreateDefaultTaskItem(priority: invalidPriority));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Constructor_WithValidPriority_CreatesSuccessfully(int validPriority)
        {
            // Arrange & Act
            var task = CreateDefaultTaskItem(priority: validPriority);

            // Assert
            Assert.Equal(validPriority, task.Priority);
        }

        [Fact]
        public void Constructor_WithPastDueDate_ThrowsDomainException()
        {
            // Arrange
            var pastDate = DateTime.UtcNow.AddYears(-1);

            // Act & Assert
            Assert.Throws<DomainException>(() => CreateDefaultTaskItem(dueDate: pastDate));
        }
        [Fact]
        public void Constructor_WithEmptyGuids_ThrowsDomainException()
        {
            // Arrange, Act & Assert
            Assert.Throws<DomainException>(() => new TaskItem("Title", "Desc", 2, null, Guid.Empty, GetValidId()));
            Assert.Throws<DomainException>(() => new TaskItem("Title", "Desc", 2, null, GetValidId(), Guid.Empty));
        }

        [Theory]
        [InlineData(TaskItemStatus.Todo, TaskItemStatus.InProgress)]
        [InlineData(TaskItemStatus.InProgress, TaskItemStatus.Done)]
        [InlineData(TaskItemStatus.Done, TaskItemStatus.InProgress)]
        [InlineData(TaskItemStatus.InProgress, TaskItemStatus.Todo)]
        public void ChangeStatus_ValidTransitions_ChangesStatusSuccessfully(TaskItemStatus initial, TaskItemStatus target)
        {
            // Arrange
            var task = CreateTaskInStatus(initial);

            // Act
            task.ChangeStatus(target);

            // Assert
            Assert.Equal(target, task.Status);
        }


        [Theory]
        [InlineData(TaskItemStatus.Todo, TaskItemStatus.Done)]
        [InlineData(TaskItemStatus.Done, TaskItemStatus.Todo)]
        [InlineData(TaskItemStatus.Todo, TaskItemStatus.Todo)]
        [InlineData(TaskItemStatus.InProgress, TaskItemStatus.InProgress)]
        [InlineData(TaskItemStatus.Done, TaskItemStatus.Done)]
        public void ChangeStatus_InvalidTransitions_ThrowsDomainException(TaskItemStatus initial, TaskItemStatus target)
        {
            // Arrange
            var task = CreateTaskInStatus(initial);

            // Act & Assert
            Assert.Throws<DomainException>(() => task.ChangeStatus(target));
            Assert.Equal(initial, task.Status); 
        }


        [Fact]
        public void ChangeStatus_ToDone_SetsCompletedAtCloseToUtcNow()
        {
            // Arrange
            var task = CreateTaskInStatus(TaskItemStatus.InProgress);

            // Act
            task.ChangeStatus(TaskItemStatus.Done);

            // Assert
            Assert.NotNull(task.CompletedAt);
            Assert.True((DateTime.UtcNow - task.CompletedAt.Value).TotalMinutes < 1);
        }

        [Fact]
        public void AssignTo_ValidId_AssignsSuccessfully()
        {
            // Arrange
            var task = CreateDefaultTaskItem();
            var assigneeId = GetValidId();

            // Act
            task.AssignTo(assigneeId);

            // Assert
            Assert.Equal(assigneeId, task.AssigneeId);
        }

        [Fact]
        public void AssignTo_EmptyId_ThrowsDomainException()
        {
            // Arrange
            var task = CreateDefaultTaskItem();

            // Act & Assert
            Assert.Throws<DomainException>(() => task.AssignTo(Guid.Empty));
            Assert.Null(task.AssigneeId);
        }

        [Fact]
        public void Unassign_ClearsAssigneeId()
        {
            // Arrange
            var task = CreateDefaultTaskItem();
            task.AssignTo(GetValidId());

            // Act
            task.Unassign();

            // Assert
            Assert.Null(task.AssigneeId);
        }



        [Fact]
        public void Update_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var task = CreateDefaultTaskItem();
            var newDueDate = DateTime.UtcNow.AddDays(10);

            // Act
            task.Update("  New Title  ", "  New Desc  ", 3, newDueDate);

            // Assert
            Assert.Equal("New Title", task.Title);
            Assert.Equal("New Desc", task.Description);
            Assert.Equal(3, task.Priority);
            Assert.Equal(newDueDate, task.DueDate);
        }

        [Fact]
        public void Update_WithInvalidTitleOrPriority_ThrowsAndKeepsOriginalData()
        {
            // Arrange
            var task = CreateDefaultTaskItem();
            var originalTitle = task.Title;
            var originalPriority = task.Priority;

            
            Assert.Throws<DomainException>(() => task.Update("ab", "Desc", 3, null));
            Assert.Equal(originalTitle, task.Title);

            
            Assert.Throws<DomainException>(() => task.Update("Valid Title", "Desc", 10, null));
            Assert.Equal(originalPriority, task.Priority);
        }

        [Fact]
        public void Update_WithPastDueDate_ThrowsDomainException()
        {
            // Arrange
            var task = CreateDefaultTaskItem();
            var pastDueDate = DateTime.UtcNow.AddDays(-5);

            // Act & Assert
            Assert.Throws<DomainException>(() => task.Update("Valid Title", "Desc", 2, pastDueDate));
        }


        [Fact]
        public void Update_WithFutureDueDate_UpdatesSuccessfully()
        {
            // Arrange
            var task = CreateDefaultTaskItem();
            var futureDueDate = DateTime.UtcNow.AddDays(30);

            // Act
            task.Update("Valid Title", "Desc", 2, futureDueDate);

            // Assert
            Assert.Equal(futureDueDate, task.DueDate);
        }






    }
}
