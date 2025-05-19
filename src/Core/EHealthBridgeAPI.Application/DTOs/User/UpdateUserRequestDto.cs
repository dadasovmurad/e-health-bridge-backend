namespace EHealthBridgeAPI.Application.DTOs
{
    public record UpdateUserRequestDto(string FirstName, string LastName,string Username, string Email);
}