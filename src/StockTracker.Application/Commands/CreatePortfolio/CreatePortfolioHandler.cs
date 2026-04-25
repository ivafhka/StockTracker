using MediatR;
using StockTracker.Application.Common;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Application.Commands.CreatePortfolio
{
    public class CreatePortfolioHandler : IRequestHandler<CreatePortfolioCommand, Result<Guid>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePortfolioHandler(
            IPortfolioRepository portfolioRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _portfolioRepository = portfolioRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result.Failure<Guid>($"User {request.UserId} not found");

            try
            {
                var portfolio = Portfolio.Create(request.UserId, request.Name, request.Description);

                await _portfolioRepository.AddAsync(portfolio, cancellationToken);
                await _unitOfWork.SaveChangeAsync(cancellationToken);

                return Result.Success(portfolio.Id);
            }
            catch(ArgumentException ex)
            {
                return Result.Failure<Guid>(ex.Message);
            }
        }
    }
}
