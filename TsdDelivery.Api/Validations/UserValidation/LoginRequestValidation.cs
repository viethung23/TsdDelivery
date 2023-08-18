using FluentValidation;
using TsdDelivery.Application.Models.User.Request;

namespace TsdDelivery.Api.Validations.UserValidation;

public class LoginRequestValidation : AbstractValidator<LoginRequest>
{
    public LoginRequestValidation()
    {
        RuleFor(x => x.PhoneNumber)
            .NotNull()
            .NotEmpty();
        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();
    }
}
