using FluentValidation;

namespace StockTracker.Application.Commands.AddPosition
{
    public class AddPositionValidator : AbstractValidator<AddPositionCommand>
    {
        public AddPositionValidator()
        {
            RuleFor(x => x.PortfolioId)
                .NotEmpty().WithMessage("Portfolio id is required");

            RuleFor(x => x.Ticker)
                .NotEmpty().WithMessage("Ticker is required")
                .MaximumLength(5);

            RuleFor(x => x.Quanity)
                .GreaterThan(0).WithMessage("Quanity must be positive");

            RuleFor(x => x.BuyPrice)
                .GreaterThan(0).WithMessage("Buy price must be positive");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currnecy is required")
                .Length(3).WithMessage("Currency must be a 3-letter ISO code");
        }
    }
}
