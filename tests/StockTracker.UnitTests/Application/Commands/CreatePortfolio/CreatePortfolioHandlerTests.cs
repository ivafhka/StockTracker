using FluentAssertions;
using NSubstitute;
using StockTracker.Application.Commands.CreatePortfolio;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;

namespace StockTracker.UnitTests.Application.Commands.CreatePortfolio
{
    public class CreatePortfolioHandlerTests
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreatePortfolioHandler _handler;

        public CreatePortfolioHandlerTests()
        {
            _portfolioRepository = Substitute.For<IPortfolioRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreatePortfolioHandler(_portfolioRepository, _userRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ShouldCreatePortfolio_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var existingUser = User.Register("tests@example.com", "hash", "Test");
            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);

            var command = new CreatePortfolioCommand(userId, "My Portfolio", "Description");
            var resut = await _handler.Handle(command, CancellationToken.None);

            resut.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

            var command = new CreatePortfolioCommand(userId, "My Portfolio", null);
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");

            await _portfolioRepository.DidNotReceive().AddAsync(Arg.Any<Portfolio>(), Arg.Any<CancellationToken>());
        }
    }
}
