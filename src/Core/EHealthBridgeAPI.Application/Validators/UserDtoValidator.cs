using EHealthBridgeAPI.Application.DTOs;
using FluentValidation;

namespace EHealthBridgeAPI.Application.Validators
{
    public class UserDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}
