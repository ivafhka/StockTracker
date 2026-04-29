using FluentValidation;

namespace StockTracker.Application.Commands.CreateAlert
{
    public class CreateAlertValidator : AbstractValidator<CreateAlertCommand>
    {
        public CreateAlertValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required");

            RuleFor(x => x.Ticker)
                .NotEmpty().WithMessage("Ticker is required")
                .MaximumLength(5);

            RuleFor(x => x.TargetPrice)
                .GreaterThan(0).WithMessage("Target price must be positive");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be a 3-letter ISO code");

            RuleFor(x => x.Direction)
                .IsInEnum().WithMessage("Direction must be Above or Below");
        }
    }
}
