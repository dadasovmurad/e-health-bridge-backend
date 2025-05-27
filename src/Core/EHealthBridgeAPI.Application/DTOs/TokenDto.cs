namespace EHealthBridgeAPI.Application.DTOs
{
    public record TokenDto(string AccessToken, DateTime Expiration, string RefreshToken);
}
