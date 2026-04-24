using MediatR;
using StockTracker.Application.Common;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Application.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (emailExists)
                return Result.Failure<Guid>($"User with email {request.Email} already exists");

            var passwordHash = _passwordHasher.Hash(request.Password);

            try
            {
                var user = User.Register(request.Email, passwordHash, request.DisplayName);

                await _userRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangeAsync(cancellationToken);

                return Result.Success(user.Id);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<Guid>(ex.Message);
            }
        }
    }
}
