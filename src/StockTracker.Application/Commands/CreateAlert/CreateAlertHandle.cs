using MediatR;
using StockTracker.Application.Common;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Application.Commands.CreateAlert;

public class CreateAlertHandler : IRequestHandler<CreateAlertCommand, Result<Guid>>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAlertHandler(
        IAlertRepository alertRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _alertRepository = alertRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<Guid>($"User {request.UserId} not found.");

        try
        {
            var ticker = Ticker.Create(request.Ticker);
            var targetPrice = Money.Create(request.TargetPrice, request.Currency);

            var alert = Alert.Create(request.UserId, ticker, targetPrice, request.Direction);

            await _alertRepository.AddAsync(alert, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(alert.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}