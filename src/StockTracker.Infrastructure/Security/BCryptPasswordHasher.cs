
using StockTracker.Application.Interfaces;
using BC = BCrypt.Net.BCrypt;
namespace StockTracker.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;
        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            return BC.HashPassword(password, WorkFactor);
        }

        public bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                return BC.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
