using StockTracker.Application.Commands.RegisterUser;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Interfaces;
using NSubstitute;
using FluentAssertions;
using StockTracker.Domain.Entities;
namespace StockTracker.UnitTests.Application.Commands.RegisterUser
{
    public class RegisterUserHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RegisterUserHandler _handler;

        public RegisterUserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordHasher = Substitute.For<IPasswordHasher>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new RegisterUserHandler(_userRepository, _passwordHasher, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ShouldRegisterUser_WhenEmailIsUnique()
        {
            var command = new RegisterUserCommand("test@example.com", "Password123", "Test User");
            _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
            _passwordHasher.Hash(command.Password).Returns("hashed_password");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenEmailAlreadyExists()
        {
            var command = new RegisterUserCommand("existing@example.com", "Password123", "Test User");
            _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("already exists");

            await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldHashPassword_BeforeSavingUser()
        {
            var command = new RegisterUserCommand("test@example.com", "Password123", "Test User");
            _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>());
            _passwordHasher.Hash("Password123").Returns("hashed_password_value");

            await _handler.Handle(command, CancellationToken.None);

            _passwordHasher.Received(1).Hash("Password123");
            await _userRepository.Received(1).AddAsync(
                Arg.Is<User>(u => u.PasswordHash == "hashed_password_value"),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenDomainValidatorFails()
        {
            var command = new RegisterUserCommand("notaemail", "Password123", "Test User");
            _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>());
            _passwordHasher.Hash(command.Password).Returns("hashed");

            var result = await _handler.Handle(command, CancellationToken.None);
            
            result.IsFailure.Should().BeTrue();
        }
    }
}
