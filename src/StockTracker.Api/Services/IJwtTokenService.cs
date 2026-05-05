using StockTracker.Domain.Entities;

namespace StockTracker.Api.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
