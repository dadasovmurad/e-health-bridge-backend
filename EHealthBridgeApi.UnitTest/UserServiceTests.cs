using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Persistence.Services;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Domain.Entities;
using Core.Results;
using AutoMapper;
using Moq;

namespace EHealthBridgeApi.UnitTest
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;
        private readonly Mock<IMapper> _mapper;
        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AppUser, AppUser>(); // Lazım olduqda dəyişdir
                cfg.CreateMap<AppUser, RegisterRequestDto>();
                cfg.CreateMap<RegisterRequestDto, AppUser>();
            });

            _mapper = new Mock<IMapper>();

            _userService = new UserService(_userRepositoryMock.Object, _mapper.Object);
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

            _mapper.Setup(m => m.Map<IEnumerable<AppUserDto>>(It.IsAny<IEnumerable<AppUser>>()))
            .Returns((IEnumerable<AppUser> users) => users.Select(u => new AppUserDto(
                u.Username,        // FirstName
                "default",         // LastName hardcoded
                u.Email,
                u.Username,        // Username (məsələn eynilə user.Username)
                "xxxx",            // PasswordHash hardcoded
                true               // IsActive hardcoded
            )).ToList());


            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
            Assert.Contains(result.Data, u => u.Username == "user1");
            Assert.Contains(result.Data, u => u.Email == "user2@mail.com");
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsSuccessResult_WhenUserExists()
        {
            // Arrange
            var email = "user1@mail.com";
            var appUser = new AppUser
            {
                Id = 1,
                Username = "user1",
                Email = email,
                FirstName = "John",
                LastName = "Doe"
            };
            //AppUserDto(string FirstName, string LastName, string Email, string Username, string PasswordHash, bool IsActive = true);
            var appUserDto = new AppUserDto("Jhon", "Doe", "user1@mail.com", "user1", "xxxx", true);

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(email))
                .ReturnsAsync(appUser);

            _mapper
                .Setup(mapper => mapper.Map<AppUserDto>(appUser))
                .Returns(appUserDto);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(email, result.Data.Email);
            Assert.Equal("user1", result.Data.Username);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsError_WhenEmailIsEmpty()
        {
            // Act
            var result = await _userService.GetByEmailAsync("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Messages.EmailCannotEmpty, result.Message);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsError_WhenUserNotFound()
        {
            // Arrange
            var email = "nonexistent@mail.com";

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(email))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Messages.EmailNotFound, result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSuccessDataResult_WhenUserExists()
        {
            // Arrange
            var testUser = new AppUser { Id = 42, Username = "test", Email = "t@test.com" };
            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(42))
                .ReturnsAsync(testUser);
            _mapper.Setup(m => m.Map<AppUserDto>(testUser))
                .Returns(new AppUserDto(testUser.Username, testUser.FirstName, testUser.Email, testUser.Username, "xxxx", true));
            // Act
            var result = await _userService.GetByIdAsync(42);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<SuccessDataResult<AppUserDto?>>(result);
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
            Assert.IsType<ErrorDataResult<AppUserDto?>>(result);
            Assert.Equal(Messages.UserNotFound, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateUser_ReturnSuccessResult_WhenCreated()
        {
            var requestedUser = new RegisterRequestDto("user1", "test", "user1@mail.com", "test", "test123");
            var expectedUser = new AppUser
            {
                Username = requestedUser.UserName,
                FirstName = requestedUser.FirstName,
                Email = requestedUser.Email,
                LastName = requestedUser.LastName,
                // Şifrə normalda hash'lənir, sadə yazdıq test üçün
                PasswordHash = requestedUser.Password
            };
            //Arrange
            _mapper.Setup(m => m.Map<AppUser>(requestedUser)).Returns(expectedUser);
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
            var expectedUser = new AppUser
            {
                Username = requestedUser.UserName,
                FirstName = requestedUser.FirstName,
                Email = requestedUser.Email,
                LastName = requestedUser.LastName,
                // Şifrə normalda hash'lənir, sadə yazdıq test üçün
                PasswordHash = requestedUser.Password
            };
            //Arrange
            _mapper.Setup(m => m.Map<AppUser>(requestedUser)).Returns(expectedUser);
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
            var expectedUser = new AppUser
            {
                Username = requestedUser.Username,
                FirstName = requestedUser.FirstName,
                Email = requestedUser.Email,
                LastName = requestedUser.LastName,
            };
            //Arrange
            _mapper.Setup(m => m.Map<AppUser>(requestedUser)).Returns(expectedUser);
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
            var expectedUser = new AppUser
            {
                Username = requestedUser.Username,
                FirstName = requestedUser.FirstName,
                Email = requestedUser.Email,
                LastName = requestedUser.LastName,
                // Şifrə normalda hash'lənir, sadə yazdıq test üçün
            };
            //Arrange
            _mapper.Setup(m => m.Map<AppUser>(requestedUser)).Returns(expectedUser);
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

            var expectedDto = new AppUserDto("user1", "test123", "user1@gmail.com", "test", "xxxx", true);

            //Arrange
            _mapper.Setup(m => m.Map<AppUserDto>(testUser)).Returns(expectedDto);
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
