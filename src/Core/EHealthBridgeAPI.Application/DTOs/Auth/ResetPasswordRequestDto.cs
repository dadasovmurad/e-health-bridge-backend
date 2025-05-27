namespace EHealthBridgeAPI.Application.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        public string? Token { get; set; }
        public string? NewPassword { get; set; }
    }
}
