using System.ComponentModel.DataAnnotations.Schema;
using EHealthBridgeAPI.Domain.Entities.Common;

namespace EHealthBridgeAPI.Domain.Entities
{
    [Table("app_users")]
    public class AppUser : BaseEntity
    {
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Şifrə sıfırlama üçün əlavə sahələr
        [Column("password_reset_token")]
        public string? PasswordResetToken { get; set; }

        [Column("password_reset_token_expiry")]
        public DateTime? PasswordResetTokenExpiry { get; set; }

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }
        [Column("refresh_token_expiration")]
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
