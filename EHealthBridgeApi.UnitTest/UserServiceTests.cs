using AutoMapper;
using Core.Results;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities;
using EHealthBridgeAPI.Persistence.Services;
using Moq;

namespace EHealthBridgeApi.UnitTest
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AppUser, AppUser>(); // Lazım olduqda dəyişdir
                cfg.CreateMap<AppUser, RegisterRequestDto>();
                cfg.CreateMap<RegisterRequestDto, AppUser>();
            });

            _mapper = config.CreateMapper();

            _userService = new UserService(_userRepositoryMock.Object, _mapper);
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
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
            // Optional: Check mapped data
            Assert.Contains(result.Data, u => u.Username == "user1");
            Assert.Contains(result.Data, u => u.Email == "user2@mail.com");
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
            Assert.True(result.IsSuccess);
            Assert.IsType<SuccessDataResult<AppUser?>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(testUser.Username, result.Data.Username);
            Assert.Equal(testUser.Email, result.Data.Email);
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
            Assert.False(result.IsSuccess);
            Assert.IsType<ErrorDataResult<AppUser?>>(result);
            Assert.Equal(Messages.UserNotFound, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateUser_ReturnSuccessResult_WhenCreated()
        {
            var requestedUser = new RegisterRequestDto("user1", "test", "user1@mail.com", "test", "test123");
            var expectedUser = _mapper.Map<AppUser>(requestedUser);
            //Arrange
            _userRepositoryMock.Setup(repo => repo.InsertAsync(expectedUser)).ReturnsAsync(1);

            var result = await _userService.CreateAsync(requestedUser);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<SuccessDataResult<int>>(result);
            Assert.Equal(Messages.Usercreated, result.Message);
        }

        [Fact]
        public async Task CreatedUser_ReturnError_WhenCanNotCreated()
        {
            var requestedUser = new RegisterRequestDto("user1", "test", "user1@mail.com", "test", "test123");
            var expectedUser = _mapper.Map<AppUser>(requestedUser);
            _userRepositoryMock.Setup(repo => repo.InsertAsync(expectedUser)).ReturnsAsync(0);

            var result = await _userService.CreateAsync(requestedUser);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ErrorDataResult<int>>(result);
            Assert.Equal(Messages.UserNotCreated, result.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnSuccess_WhenUpdate()
        {
            var requestedUser = new UpdateUserRequestDto("user1", "test", "user1@mail.com", "test");
            var expectedUser = _mapper.Map<AppUser>(requestedUser);
            expectedUser.Id = 1; // Set the Id for the user to be updated

            //Arrange
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(expectedUser)).ReturnsAsync(true);
            _userRepositoryMock
       .Setup(repo => repo.GetByIdAsync(1))
       .ReturnsAsync(expectedUser);

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(true);

            var result = await _userService.UpdateAsync(expectedUser.Id, requestedUser);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<SuccessResult>(result);
            Assert.Equal(Messages.UserSuccessfullyUpdated, result.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnError_WhenUpdateFails()
        {
            var requestedUser = new UpdateUserRequestDto("user1", "test", "user1@mail.com", "test");
            var expectedUser = _mapper.Map<AppUser>(requestedUser);
            expectedUser.Id = 1; // Set the Id for the user to be updated

            //Arrange
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(expectedUser)).ReturnsAsync(true);
            _userRepositoryMock
       .Setup(repo => repo.GetByIdAsync(1))
       .ReturnsAsync(expectedUser);

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(expectedUser))
                .ReturnsAsync(false);

            var result = await _userService.UpdateAsync(1, requestedUser);

            Assert.False(result.IsSuccess);
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
            Assert.True(result.IsSuccess);
            Assert.IsType<SuccessResult>(result);
            Assert.Equal(Messages.UserDeleted, result.Message);
        }

        [Fact]
        public async Task GetByUsernameAsync_ReturnsError_WhenUsernameIsEmpty()
        {
            // Act
            var result = await _userService.GetByUsernameAsync("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ErrorDataResult<AppUserDto>>(result);
            Assert.Equal(Messages.InvalidRequest, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByUsernameAsync_ReturnsSuccess_WhenUserExists()
        {
            // Arrange
            var testUser = new AppUser { Id = 1, Username = "user1", Email = "user1@mail.com" };
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("user1"))
                               .ReturnsAsync(testUser);

            var expectedDto = _mapper.Map<AppUserDto>(testUser);

            // Act
            var result = await _userService.GetByUsernameAsync("user1");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<SuccessDataResult<AppUserDto>>(result);
            Assert.Equal(expectedDto.Username, result.Data.Username);
        }

        [Fact]
        public async Task GetByUsernameAsync_ReturnsError_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("unknownUser"))
                               .ReturnsAsync((AppUser)null);

            // Act
            var result = await _userService.GetByUsernameAsync("unknownUser");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ErrorDataResult<AppUserDto>>(result);
            Assert.Equal(Messages.UserNotFound, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByUsernameAsync_ReturnsError_WhenUsernameIsNull()
        {
            // Act
            var result = await _userService.GetByUsernameAsync(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ErrorDataResult<AppUserDto>>(result);
            Assert.Equal(Messages.InvalidRequest, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByUsernameAsync_ReturnsError_WhenUsernameIsWhitespace()
        {
            // Act
            var result = await _userService.GetByUsernameAsync("   ");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ErrorDataResult<AppUserDto>>(result);
            Assert.Equal(Messages.InvalidRequest, result.Message);
            Assert.Null(result.Data);
        }
    }
}
