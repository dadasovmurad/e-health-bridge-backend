using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Results;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities;
using EHealthBridgeAPI.Persistence.Services;
using Moq;

namespace EHealthBridgeApi.UnitTest
{
    public class UserServiceTests
    {
        private readonly Mock<IGenericRepository<AppUser>> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IGenericRepository<AppUser>>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSuccessResult_WithUsers()
        {
            // Arrange
            var expectedUsers = new List<AppUser>
        {
            new AppUser { Id = 1, Username = "user1", Email = "user1@mail.com" },
            new AppUser { Id = 2, Username = "user2", Email = "user2@mail.com" }
        };
            _userRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSuccessDataResult_WhenUserExists()
        {
            // Arrange
            var testUser = new AppUser { Id = 42, Username = "test", Email = "t@test.com" };
            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(42))
                .ReturnsAsync(testUser);

            // Act
            var result = await _userService.GetByIdAsync(42);

            // Assert
            Assert.True(result.Success);
            Assert.IsType<SuccessDataResult<AppUser?>>(result);
            Assert.Equal(testUser, result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsErrorDataResult_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((AppUser?)null);

            // Act
            var result = await _userService.GetByIdAsync(123);

            // Assert
            Assert.False(result.Success);
            Assert.IsType<ErrorDataResult<AppUser?>>(result);
            Assert.Equal(Messages.UserNotFound, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateUser_ReturnSuccessResult_WhenCreated()
        {
            var expectedUsers = new AppUser { Id = 1, Username = "user1", Email = "user1@mail.com" };
            //Arrange
            _userRepositoryMock.Setup(repo => repo.InsertAsync(expectedUsers)).ReturnsAsync(1);

            var result = await _userService.CreateAsync(expectedUsers);

            // Assert
            Assert.True(result.Success);
            Assert.IsType<SuccessDataResult<int>>(result);
            Assert.Equal(Messages.Usercreated, result.Message);
        }

        [Fact]
        public async Task CreatedUser_ReturnError_WhenCanNotCreated()
        {
            var expectedUsers = new AppUser { Username = "failuser", Email = "fail@example.com" };
            _userRepositoryMock.Setup(repo => repo.InsertAsync(expectedUsers)).ReturnsAsync(0);

            var result = await _userService.CreateAsync(expectedUsers);

            // Assert
            Assert.False(result.Success);
            Assert.IsType<ErrorDataResult<int>>(result);
            Assert.Equal(Messages.UserNotCreated, result.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnSuccess_WhenUpdate()
        {
            var expectedUsers = new AppUser { Id = 1, Username = "user1", Email = "user1@mail.com" };

            //Arrange
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(expectedUsers)).ReturnsAsync(true);

            var result = await _userService.UpdateAsync(expectedUsers);

            // Assert
            Assert.True(result.Success);
            Assert.IsType<SuccessResult>(result);
            Assert.Equal(Messages.UserUpdated, result.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnError_WhenUpdateFails()
        {
            var expectedUser = new AppUser { Id = 1, Username = "user1", Email = "user1@mail.com" };

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(expectedUser))
                .ReturnsAsync(false);

            var result = await _userService.UpdateAsync(expectedUser);

            Assert.False(result.Success);
            Assert.IsType<ErrorResult>(result);
            Assert.Equal(Messages.UserNotUpdated, result.Message);
        }

        [Fact]
        public async Task RemoveUserById_ReturnSuccess_WhenRemoved()
        {
            // Arrange
            var testUser = new AppUser { Id = 42, Username = "test", Email = "t@test.com" };
            _userRepositoryMock
                .Setup(repo => repo.DeleteAsync(42))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.RemoveByIdAsync(42);
            // Assert
            Assert.True(result.Success);
            Assert.IsType<SuccessResult>(result);
            Assert.Equal(Messages.UserDeleted, result.Message);
        }
    }
}
