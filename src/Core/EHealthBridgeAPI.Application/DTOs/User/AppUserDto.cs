namespace EHealthBridgeAPI.Application.DTOs.User
{
    public record AppUserDto(string FirstName, string LastName, string Email, string Username, string PasswordHash, bool IsActive = true);
}
