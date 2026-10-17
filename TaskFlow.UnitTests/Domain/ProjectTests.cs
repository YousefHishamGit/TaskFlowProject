using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using Xunit;

namespace TaskFlow.UnitTests.Domain
{
    public class ProjectTests
    {
        private static Guid GetValidOwnerId() => Guid.NewGuid();
        private static Project CreateDefaultProject() =>
            new Project("Website", "Default description", GetValidOwnerId());


        [Fact]
        public void Constructor_WithValidData_CreatesProjectSuccessfully()
        {
            // Arrange
            var name = "  Website  ";
            var description = "  desc  ";
            var ownerId = GetValidOwnerId();

            // Act
            var project = new Project(name, description, ownerId);

            // Assert
            Assert.Equal("Website", project.Name);
            Assert.Equal("desc", project.Description);
            Assert.Equal(ownerId, project.OwnerId);
            Assert.NotEqual(Guid.Empty, project.Id);
        }

        [Fact]
        public void Constructor_WithNullDescription_StoresEmptyString()
        {
            // Arrange & Act
            var project = new Project("Website", null, GetValidOwnerId());

            // Assert
            Assert.NotNull(project.Description);
            Assert.Equal(string.Empty, project.Description);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ab")]
        public void Constructor_WithInvalidName_ThrowsDomainException(string invalidName)
        {
            // check the exception class
            Assert.Throws<DomainException>(() => new Project(invalidName, "desc", GetValidOwnerId()));
        }

        [Fact]
        public void Constructor_WithEmptyOwnerId_ThrowsDomainException()
        {
            // Arrange, Act & Assert
            Assert.Throws<DomainException>(() => new Project("Website", "desc", Guid.Empty));
        }


        [Fact]
        public void Constructor_AddsOwnerAsMemberAutomatically()
        {
            // Arrange
            var ownerId = GetValidOwnerId();

            // Act
            var project = new Project("Website", "desc", ownerId);

            // Assert
            Assert.Single(project.Members); 
            Assert.True(project.IsMember(ownerId));
        }

        [Fact]
        public void AddMember_WithValidId_AddsMemberSuccessfully()
        {
            // Arrange
            var project = CreateDefaultProject();
            var newMemberId = Guid.NewGuid();

            // Act
            project.AddMember(newMemberId);

            // Assert
            Assert.Equal(2, project.Members.Count);
            Assert.True(project.IsMember(newMemberId));
        }

        [Fact]
        public void AddMember_AlreadyExistingOrEmpty_ThrowsDomainException()
        {
            // Arrange
            var project = CreateDefaultProject();
            var memberId = Guid.NewGuid();
            project.AddMember(memberId);//now we have 2 members (owner and member)

            // Act & Assert 
            Assert.Throws<DomainException>(() => project.AddMember(memberId));//same id of second
            Assert.Throws<DomainException>(() => project.AddMember(Guid.Empty));
        }


        [Fact]
        public void RemoveMember_ValidMember_RemovesSuccessfully()
        {
            // Arrange
            var project = CreateDefaultProject();
            var memberId = Guid.NewGuid();
            project.AddMember(memberId);

            // Act
            project.RemoveMember(memberId);

            // Assert
            Assert.Single(project.Members);
            Assert.False(project.IsMember(memberId));
        }

        [Fact]
        public void RemoveMember_OwnerOrNonExisting_ThrowsDomainException()
        {
            // Arrange
            var project = CreateDefaultProject();
            var ownerId = project.OwnerId;
            var randomUserId = Guid.NewGuid();

            // Act & Assert 
            Assert.Throws<DomainException>(() => project.RemoveMember(ownerId));
            Assert.True(project.IsMember(ownerId)); 

            Assert.Throws<DomainException>(() => project.RemoveMember(randomUserId));
        }

        [Fact]
        public void IsMember_WithNonMemberId_ReturnsFalse()
        {
            // Arrange
            var project = CreateDefaultProject();

            // Act & Assert
            Assert.False(project.IsMember(Guid.NewGuid()));
        }



        [Fact]
        public void Update_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var project = CreateDefaultProject();

            // Act
            project.Update("  New Name  ", "  New Desc  ");

            // Assert
            Assert.Equal("New Name", project.Name);
            Assert.Equal("New Desc", project.Description);
        }

        [Fact]
        public void Update_WithInvalidName_DoesNotChangeOriginalData()
        {
            // Arrange
            var project = CreateDefaultProject();
            var oldName = project.Name;

            // Act & Assert
            Assert.Throws<DomainException>(() => project.Update("ab", "New Desc"));
            Assert.Equal(oldName, project.Name); 
        }
    }
}
