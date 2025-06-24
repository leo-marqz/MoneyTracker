
using FluentValidation;
using MoneyTracker.DTOs;

namespace MoneyTracker.Validations
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationValidator()
        {
            RuleFor((field) => field.Firstname)
                .NotEmpty().WithMessage("Firstname is required.")
                .Length(2, 50).WithMessage("Firstname must be between 2 and 50 characters.");
            RuleFor((field) => field.Lastname)
                .NotEmpty().WithMessage("Lastname is required.")
                .Length(2, 50).WithMessage("Lastname must be between 2 and 50 characters.");
            RuleFor((field) => field.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor((field) => field.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W]").WithMessage("Password must contain at least one special character.");
            RuleFor((field) => field.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(field => field.Password).WithMessage("Passwords do not match.");
        }
    }
}