using FluentValidation;

namespace StockTracker.Application.Queries.GetPriceHistory
{
    public class GetPriceHistoryValidator : AbstractValidator<GetPriceHistoryQuery>
    {
        public GetPriceHistoryValidator()
        {
            RuleFor(x => x.Ticker)
                .NotEmpty().WithMessage("Ticker is required")
                .MaximumLength(5);

            RuleFor(x => x.From)
                .LessThan(x => x.To).WithMessage("From date must be before To date");

            RuleFor(x => x.To)
                .LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("To date cannto be in the future");

            RuleFor(x => x)
                .Must(x => (x.To - x.From).TotalDays <= 365)
                .WithMessage("Date renge cannot exceed 1 year");
        }
    }
}
