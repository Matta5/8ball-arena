using Moq;
using BLL;
using BLL.Interfaces;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Exceptions.Duel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DuelServiceTests
{
    [TestClass]
    public class DuelServiceTests
    {
        private Mock<IDuelRepository> _duelRepositoryMock;
        private DuelService _duelService;
        private Mock<ICommentRepository> _commentRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _duelRepositoryMock = new Mock<IDuelRepository>();

            _commentRepositoryMock = new Mock<ICommentRepository>();
            _duelService = new DuelService(_duelRepositoryMock.Object, _commentRepositoryMock.Object);
        }

        [TestMethod]
        public void GetDuelById_DuelExists_ShouldReturnDuel()
        {
            // Arrange
            var duel = new DuelDTO { Id = 1, Status = "Pending" };
            _duelRepositoryMock.Setup(repo => repo.GetDuelById(1)).Returns(duel);

            // Act
            var result = _duelService.GetDuelById(1);

            // Assert
            Assert.AreEqual(duel, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetDuelById_DuelDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            _duelRepositoryMock.Setup(repo => repo.GetDuelById(1)).Throws(new NotFoundException());

            // Act
            _duelService.GetDuelById(1);
        }

        [TestMethod]
        [ExpectedException(typeof(DuelServiceException))]
        public void GetDuelById_RepositoryException_ShouldThrowDuelServiceException()
        {
            // Arrange
            _duelRepositoryMock.Setup(repo => repo.GetDuelById(1)).Throws(new DuelRepositoryException(""));

            // Act
            _duelService.GetDuelById(1);
        }

        [TestMethod]
        public void GetDuelsByUserId_UserHasDuels_ShouldReturnDuels()
        {
            // Arrange
            var duels = new List<DuelDTO> { new DuelDTO { Id = 1, Status = "Pending" } };
            _duelRepositoryMock.Setup(repo => repo.GetDuelsByUserId(1)).Returns(duels);

            // Act
            var result = _duelService.GetDuelsByUserId(1);

            // Assert
            CollectionAssert.AreEqual(duels, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetDuelsByUserId_NoDuels_ShouldThrowNotFoundException()
        {
            // Arrange
            _duelRepositoryMock.Setup(repo => repo.GetDuelsByUserId(1)).Throws(new NotFoundException());

            // Act
            _duelService.GetDuelsByUserId(1);
        }

        [TestMethod]
        [ExpectedException(typeof(DuelServiceException))]
        public void GetDuelsByUserId_RepositoryException_ShouldThrowDuelServiceException()
        {
            // Arrange
            _duelRepositoryMock.Setup(repo => repo.GetDuelsByUserId(1)).Throws(new DuelRepositoryException(""));

            // Act
            _duelService.GetDuelsByUserId(1);
        }

        [TestMethod]
        public void CreateDuel_ValidUsers_ShouldReturnDuelId()
        {
            // Arrange
            var userId1 = 1;
            var userId2 = 2;
            var duelId = 1;
            _duelRepositoryMock.Setup(repo => repo.CreateDuel(userId1, userId2)).Returns(duelId);

            // Act
            var result = _duelService.CreateDuel(userId1, userId2);

            // Assert
            Assert.AreEqual(duelId, result);
        }

        [TestMethod]
        [ExpectedException(typeof(DuelServiceException))]
        public void CreateDuel_SameUser_ShouldThrowDuelServiceException()
        {
            // Arrange
            var userId = 1;

            // Act
            _duelService.CreateDuel(userId, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(DuelServiceException))]
        public void CreateDuel_RepositoryException_ShouldThrowDuelServiceException()
        {
            // Arrange
            var userId1 = 1;
            var userId2 = 2;
            _duelRepositoryMock.Setup(repo => repo.CreateDuel(userId1, userId2)).Throws(new DuelRepositoryException(""));

            // Act
            _duelService.CreateDuel(userId1, userId2);
        }

        [TestMethod]
        public void AssignWinner_ValidData_ShouldAssignWinner()
        {
            // Arrange
            var duelId = 1;
            var winnerId = 2;

            // Act
            _duelService.AssignWinner(duelId, winnerId);

            // Assert
            _duelRepositoryMock.Verify(repo => repo.AssignWinner(duelId, winnerId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void AssignWinner_DuelOrUserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var duelId = 1;
            var winnerId = 2;
            _duelRepositoryMock.Setup(repo => repo.AssignWinner(duelId, winnerId)).Throws(new NotFoundException());

            // Act
            _duelService.AssignWinner(duelId, winnerId);
        }

        [TestMethod]
        [ExpectedException(typeof(DuelServiceException))]
        public void AssignWinner_RepositoryException_ShouldThrowDuelServiceException()
        {
            // Arrange
            var duelId = 1;
            var winnerId = 2;
            _duelRepositoryMock.Setup(repo => repo.AssignWinner(duelId, winnerId)).Throws(new DuelRepositoryException(""));

            // Act
            _duelService.AssignWinner(duelId, winnerId);
        }
    }
}
