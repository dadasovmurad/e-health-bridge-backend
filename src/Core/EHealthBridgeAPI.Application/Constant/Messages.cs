namespace EHealthBridgeAPI.Application.Constant
{
    public static class Messages
    {
        public const string UserNotFound = "User not found";
        public const string LoginSuccess = "Successfully logged in";
        public const string LoginFailure = "Incorrect username or password";

        public const string InvalidRequest = "Invalid request";

        public const string UserNotCreated = "User registration failed";
        public const string Usercreated = "User registered successfully";

        public const string UserSuccessfullyUpdated = "User updated successfully";
        public const string UserNotUpdated = "User update failed";

        public const string UserDeleted = "User deleted successfully";
        public const string UserNotDeleted = "User could not be deleted";

        public const string UserAlreadyExists = "User already exists";

        // ✅ Password reset mesajları
        public const string PasswordResetTokenCreated = "Password reset token successfully generated";
        public const string PasswordResetTokenInvalid = "Invalid or expired reset token";
        public const string PasswordResetSuccess = "Password successfully reset";
        public const string PasswordResetTokenExpired = "Reset token has expired";
        public const string EmailCannotEmpty = "Email cannot be empty";
        public const string EmailNotFound = "Email not found";
        public const string PasswordCannotBeEmpty = "Password cannot be empty";
        public const string TokenCannotBeEmpty = "Reset token cannot be empty";
        public const string TokenOrPasswordCannotBeEmpty = "Token or new password cannot be empty.";
        public const string InvalidResetToken = "The reset token is invalid.";
        public const string ResetTokenExpired = "The reset token has expired.";
        public const string RefreshTokenExpired = "The refresh token is invalid.";
    }
}