using Moq;
using BLL;
using BLL.Interfaces;
using BLL.DTOs;
using BLL.Exceptions;

namespace UserServiceTests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [TestMethod]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<UserDTO> { new UserDTO { Id = 1, Username = "testuser" } };
            _userRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(users);

            // Act
            var result = _userService.GetAllUsers();

            // Assert
            Assert.AreEqual(users, result);
        }

        [TestMethod]
        public void GetUserById_UserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new UserDTO { Id = 1, Username = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Act
            var result = _userService.GetUserById(1);

            // Assert
            Assert.AreEqual(user, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetUserById_UserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns((UserDTO)null);

            // Act
            _userService.GetUserById(1);
        }

        [TestMethod]
        public void CreateUser_InvalidPassword_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "short" }; // Assuming "short" is an invalid password
            _userRepositoryMock.Setup(repo => repo.CheckIfUsernameExists(user.Username)).Returns(false);

            // Act & Assert
            var ex = Assert.ThrowsException<UserServiceException>(() => _userService.CreateUser(user));
        }

        [TestMethod]
        public void CreateUser_ValidUser_ShouldCreateUser()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "Password1" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);

            // Act
            var result = _userService.CreateUser(user);

            // Assert
            Assert.IsTrue(result);
            _userRepositoryMock.Verify(repo => repo.CreateUser(user), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void CreateUser_WeakPassword_ShouldThrowException()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "weak" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);

            // Act
            _userService.CreateUser(user);
        }

        [TestMethod]
        public void CreateUser_DuplicateUsername_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "Password1" };
            _userRepositoryMock.Setup(repo => repo.CheckIfUsernameExists(user.Username)).Returns(true);

            // Act & Assert
            var ex = Assert.ThrowsException<UserServiceException>(() => _userService.CreateUser(user));
            Assert.AreEqual("Username already exists", ex.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void CreateUser_NullUser_ShouldThrowException()
        {
            // Act
            _userService.CreateUser(null);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void EditUser_UsernameExists_ShouldThrowDuplicateException()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns(new UserDTO());

            // Act
            _userService.EditUser(1, user);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void EditUser_UserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns((UserDTO)null);

            // Act
            _userService.EditUser(1, user);
        }

        [TestMethod]
        public void EditUser_ValidUser_ShouldEditUser()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser" };
            var existingUser = new UserDTO { Id = 1, Username = "existinguser" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(existingUser);

            // Act
            _userService.EditUser(1, user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.EditUser(1, user), Times.Once);
        }
    }
}
