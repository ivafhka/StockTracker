using MediatR;
using StockTracker.Application.Common;

namespace StockTracker.Application.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string Email,
        string Password,
        string DisplayName) : IRequest<Result<Guid>>;
}
