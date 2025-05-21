using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Domain.Entities;
using Core.Results;
using AutoMapper;

namespace EHealthBridgeAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<IEnumerable<AppUserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<AppUserDto>>(users);

            if (users == null || !users.Any())
            {
                return new ErrorDataResult<IEnumerable<AppUserDto>>(Messages.UserNotFound);
            }

            return new SuccessDataResult<IEnumerable<AppUserDto>>(result);
        }

        public async Task<IDataResult<AppUserDto>> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new ErrorDataResult<AppUserDto>(Messages.InvalidRequest);
            }

            var requestUser = await _userRepository.GetByUsernameAsync(username);
            if (requestUser is null)
            {
                return new ErrorDataResult<AppUserDto>(Messages.UserNotFound);
            }

            var appUserDto = _mapper.Map<AppUserDto>(requestUser);
            return new SuccessDataResult<AppUserDto>(appUserDto);
        }

        public async Task<IDataResult<AppUserDto>> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ErrorDataResult<AppUserDto>(Messages.EmailCannotEmpty);
            }

            var user = await _userRepository.GetByEmailAsync(email); // repository-də olmalıdır
            if (user == null)
            {
                return new ErrorDataResult<AppUserDto>(Messages.EmailNotFound);
            }

            var appUserDto = _mapper.Map<AppUserDto>(user);
            return new SuccessDataResult<AppUserDto>(appUserDto);
        }

        public async Task<IResult> GeneratePasswordResetTokenAsync(string email)
        {
            var userResult = await GetByEmailAsync(email);
            if (!userResult.IsSuccess)
                return new ErrorResult(userResult.Message);

            var user = await _userRepository.GetByEmailAsync(email);

            var token = Guid.NewGuid().ToString(); // Sadə token, JWT də istifadə oluna bilər
            var expiry = DateTime.UtcNow.AddMinutes(30);

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiry;

            await _userRepository.UpdateAsync(user);

            // Əgər mail göndərmək olsaydı, burada göndəriləcəkdi

            return new SuccessResult(Messages.PasswordResetTokenCreated);
        }

        public async Task<IResult> ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
                return new ErrorResult(Messages.TokenOrPasswordCannotBeEmpty);

            var user = await _userRepository.GetByResetTokenAsync(token); // bu metod aşağıda izah olunub
            if (user == null)
                return new ErrorResult(Messages.InvalidResetToken);

            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return new ErrorResult(Messages.ResetTokenExpired);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = DateTime.MinValue;

            await _userRepository.UpdateAsync(user);
            //burada emaile token gonderile biler
           // var resetLink = $"https://yourapp.com/reset-password?token={token}";
            return new SuccessResult(Messages.PasswordResetSuccess);
        }

        public async Task<IDataResult<AppUserDto>> GetByIdAsync(int id)
        {
            var userById = await _userRepository.GetByIdAsync(id);
            if (userById is null)
            {
                return new ErrorDataResult<AppUserDto>(Messages.UserNotFound);
            }
            var appUserDto = _mapper.Map<AppUserDto>(userById);

            return new SuccessDataResult<AppUserDto>(appUserDto);
        }

        public async Task<IDataResult<int>> CreateAsync(RegisterRequestDto registerRequestDto)
        {
            var requestUser = await _userRepository.GetByUsernameAsync(registerRequestDto.UserName);
            if (requestUser is not null)
            {
                return new ErrorDataResult<int>(Messages.UserAlreadyExists);
            }

            var newuser= _mapper.Map<AppUser>(registerRequestDto);
            newuser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password);

            var token = Guid.NewGuid().ToString(); // Sadə token, JWT də istifadə oluna bilər
            var expiry = DateTime.UtcNow.AddMinutes(30);

            newuser.PasswordResetToken = token;
            newuser.PasswordResetTokenExpiry = expiry;
            var createdUser = await _userRepository.InsertAsync(newuser);

            if (createdUser == 0)
            {
                return new ErrorDataResult<int>(Messages.UserNotCreated);
            }
            return new SuccessDataResult<int>(createdUser, Messages.Usercreated);
        }

        public async Task<IResult> UpdateAsync(int id, UpdateUserRequestDto updateUserRequestDto)
        {
            var userById = await _userRepository.GetByIdAsync(id);
            if (userById is not null)
            {
                var User = _mapper.Map<AppUser>(updateUserRequestDto);
                User.Id = id;
                var updatedStatus = await _userRepository.UpdateAsync(User);
                if (updatedStatus)
                {
                    return new SuccessResult(Messages.UserSuccessfullyUpdated);
                }
                return new ErrorResult(Messages.UserNotUpdated);
            }
            return new ErrorResult(Messages.UserNotFound);
        }

        public async Task<IResult> RemoveByIdAsync(int id)
        {
            var deletedStatus = await _userRepository.DeleteAsync(id);
            if (!deletedStatus)
            {
                return new ErrorResult(Messages.UserNotDeleted);
            }
            return new SuccessResult(Messages.UserDeleted);
        }
    }
}