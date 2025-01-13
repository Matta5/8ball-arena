using Moq;
using BLL;
using BLL.Interfaces;
using BLL.DTOs;
using BLL.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using BLL.Exceptions.User;

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
        [ExpectedException(typeof(UserServiceException))]
        public void GetAllUsers_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetAllUsers()).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.GetAllUsers();
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
        [ExpectedException(typeof(UserServiceException))]
        public void GetUserById_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.GetUserById(1);
        }

        [TestMethod]
        public void GetUserByUsername_UserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new UserDTO { Id = 1, Username = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns(user);

            // Act
            var result = _userService.GetUserByUsername("testuser");

            // Assert
            Assert.AreEqual(user, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetUserByUsername_UserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Returns((UserDTO)null);

            // Act
            _userService.GetUserByUsername("testuser");
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void GetUserByUsername_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername("testuser")).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.GetUserByUsername("testuser");
        }

        [TestMethod]
        public void ValidateUserCredentials_ValidCredentials_ShouldReturnTrue()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.ValidateUserCredentials("testuser", "Password1", out userId)).Returns(true);

            // Act
            var result = _userService.ValidateUserCredentials("testuser", "Password1", out userId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void ValidateUserCredentials_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.ValidateUserCredentials("testuser", "Password1", out userId)).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.ValidateUserCredentials("testuser", "Password1", out userId);
        }

        [TestMethod]
        public void CreateUser_ValidUser_ShouldCreateUser()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "Password1", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.CheckIfUsernameExists(user.Username)).Returns(false);

            // Act
            var result = _userService.CreateUser(user);

            // Assert
            Assert.IsTrue(result);
            _userRepositoryMock.Verify(repo => repo.CreateUser(user), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void CreateUser_DuplicateUsername_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "Password1", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.CheckIfUsernameExists(user.Username)).Returns(true);

            // Act
            _userService.CreateUser(user);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void CreateUser_InvalidPassword_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "short", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.CheckIfUsernameExists(user.Username)).Returns(false);

            // Act
            _userService.CreateUser(user);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void CreateUser_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new CreateUserDTO { Username = "testuser", Password = "Password1", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.CheckIfUsernameExists(user.Username)).Returns(false);
            _userRepositoryMock.Setup(repo => repo.CreateUser(user)).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.CreateUser(user);
        }

        [TestMethod]
        public void EditUser_ValidUser_ShouldEditUser()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser", Email = "test@example.com" };
            var existingUser = new UserDTO { Id = 1, Username = "existinguser" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(existingUser);

            // Act
            _userService.EditUser(1, user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.EditUser(1, user), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void EditUser_DuplicateUsername_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns(new UserDTO());

            // Act
            _userService.EditUser(1, user);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void EditUser_UserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns((UserDTO)null);

            // Act
            _userService.EditUser(1, user);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void EditUser_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            var user = new EditUserDTO { Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(user.Username)).Returns((UserDTO)null);
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(new UserDTO());
            _userRepositoryMock.Setup(repo => repo.EditUser(1, user)).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.EditUser(1, user);
        }

        [TestMethod]
        public void DeleteUser_ValidUser_ShouldDeleteUser()
        {
            // Arrange
            var existingUser = new UserDTO { Id = 1, Username = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(existingUser);

            // Act
            _userService.DeleteUser(1);

            // Assert
            _userRepositoryMock.Verify(repo => repo.DeleteUser(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void DeleteUser_RepositoryException_ShouldThrowUserServiceException()
        {
            // Arrange
            var existingUser = new UserDTO { Id = 1, Username = "testuser" };
            _userRepositoryMock.Setup(repo => repo.GetUserById(1)).Returns(existingUser);
            _userRepositoryMock.Setup(repo => repo.DeleteUser(1)).Throws(new UserRepositoryException("Error"));

            // Act
            _userService.DeleteUser(1);
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void ValidatePassword_EmptyPassword_ShouldThrowUserServiceException()
        {
            // Act
            _userService.ValidatePassword("");
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void ValidatePassword_NoUpperCase_ShouldThrowUserServiceException()
        {
            // Act
            _userService.ValidatePassword("password1");
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void ValidatePassword_NoNumber_ShouldThrowUserServiceException()
        {
            // Act
            _userService.ValidatePassword("Password");
        }

        [TestMethod]
        [ExpectedException(typeof(UserServiceException))]
        public void ValidatePassword_TooShort_ShouldThrowUserServiceException()
        {
            // Act
            _userService.ValidatePassword("Pass1");
        }

        [TestMethod]
        public void ValidatePassword_ValidPassword_ShouldNotThrowException()
        {
            // Act
            _userService.ValidatePassword("Password1");
        }
    }
}
