using FluentValidation;

namespace StockTracker.Application.Commands.CreatePortfolio
{
    public class CreatePortfolioValidator : AbstractValidator<CreatePortfolioCommand>
    {
        public CreatePortfolioValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Poftfolio name is required")
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x=>x.Description)
                .MaximumLength(500)
                .When(x=>!string.IsNullOrEmpty(x.Description));
        }
    }
}
