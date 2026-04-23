using StockTracker.Domain.Common;

namespace StockTracker.Domain.Entities
{
    public class User : Entity
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string DisplayName { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User(string email, string passwordHash, string displayName)
        {
            Email = email;
            PasswordHash = passwordHash;
            DisplayName = displayName;
            CreatedAt = DateTime.UtcNow;
        }

        private User() { }

        public static User Register(string email, string passwordHash, string displayName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (!email.Contains("@"))
                throw new ArgumentException("Invalid email format", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is reqired", nameof(passwordHash));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name is required", nameof(displayName));

            return new User(email.ToLowerInvariant(), passwordHash, displayName);
        }

        public void UpdateDisplayName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Display name cannot be empty", nameof(newName));

            DisplayName = newName;
        }

    }
}
