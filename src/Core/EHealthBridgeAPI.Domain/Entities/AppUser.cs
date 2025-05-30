using System.ComponentModel.DataAnnotations.Schema;
using EHealthBridgeAPI.Domain.Entities.Common;

namespace EHealthBridgeAPI.Domain.Entities
{
    [Table("app_users")]
    public class AppUser : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;

        // Şifrə sıfırlama üçün əlavə sahələr
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }

        public AppUser() { }

        public AppUser(int id, string username, string email, string passwordHash,
                   string firstName, string lastName)
        {
            Id = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            IsActive = true;
        }
    }
}
