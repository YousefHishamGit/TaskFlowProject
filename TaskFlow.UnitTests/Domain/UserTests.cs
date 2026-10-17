
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using Xunit;

namespace TaskFlow.UnitTests.Domain
{
    /*
     * Each unit test 
     *          Decompose into 3 things (AAA)
     *                  1-Arrange => prepare what you want to test
     *                  2-Act     => make the test or action on what you prepare  
     *                  3-Assert  => check the result 
     *        
     * Each test is Method which has attr. [Fact] or [Theory]       
     */
    public class UserTests
    {
        [Fact] // only one test with constant value 
        public void Constructor_WithValidData_CreatesUserSuccessfully()
        {
            //Arrange
            var fullName = "Ahmed";
            var email = "ahmed@mail.com";
            var passwordHash = "hash123";
            //Act
            var user = new User(fullName, email, passwordHash);
            //Assert
            Assert.NotNull(user);
            Assert.Equal(fullName, user.FullName);
            Assert.Equal(email, user.Email);
            Assert.Equal(passwordHash, user.PasswordHash);
            Assert.Equal(UserRole.Member, user.Role);
            Assert.NotEqual(Guid.Empty, user.Id);
        }

        [Fact]
        public void Constructor_GeneratesUniqueIdsForDifferentUsers()
        {
            //Act
            var user1 = new User("Name1", "email1@gmail.com", "hash123");
            var user2 = new User("Name2", "email2@gmail.com", "hash456");
            //assert
            Assert.NotEqual(Guid.Empty,user1.Id);
            Assert.NotEqual(Guid.Empty ,user2.Id);
            Assert.NotEqual(user1.Id,user2.Id);
        }



        [Theory] // to test many case not one test only 
        [InlineData(null)] // test 1
        [InlineData("")] // test 2
        [InlineData("   ")] // test 3
        public void Constructor_WithInvalidFullName_ThrowsDomainException(string invalidFullName)
        {
            // Arrange & Act 
            var exception = Assert.Throws<DomainException>(() => new User(invalidFullName, "ahmed@mail.com", "hash123"));
            //Assert    
            Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
        }




        [Theory] // to test many case not one test only 
        [InlineData(null)] // test 1
        [InlineData("")] // test 2
        [InlineData("   ")] // test 3
        public void Constructor_WithInvalidEmail_ThrowsDomainException(string invalidEmail)
        {
            // Arrange & Act 
            var exception = Assert.Throws<DomainException>(() => new User("Ahmed", invalidEmail, "hash123"));
            //Assert    
            Assert.Contains("email", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
