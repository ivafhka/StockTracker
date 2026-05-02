
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;
using StockTracker.Infrastructure.Persistence.Repositories;

namespace StockTracker.IntegrationTests.Persistence
{
    [Collection(nameof(PostgresCollection))]
    public class UserRepositoryTests : PersistenceTestBase
    {
        public UserRepositoryTests(PostgresFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Addasync_ShouldPersistUser()
        {
            await CleanupAsync();
            var repository = new UserRepository(DbContext);
            var user = User.Register("test@example.com", "hash123", "Test User");

            await repository.AddAsync(user);
            await DbContext.SaveChangesAsync();

            var saved = await repository.GetByIdAsync(user.Id);
            saved.Should().NotBeNull();
            saved!.Email.Should().Be("test@example.com");
            saved.DisplayName.Should().Be("Test User");
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldFindUser_RegardlessOfCase()
        {
            await CleanupAsync();
            var repository = new UserRepository(DbContext);
            var user = User.Register("Test@Example.com", "hash", "User");
            await repository.AddAsync(user);
            await DbContext.SaveChangesAsync();

            var found = await repository.GetByEmailAsync("test@example.com");
            var foundUpper = await repository.GetByEmailAsync("TEST@EXAMPLE.COM");

            found.Should().NotBeNull();
            foundUpper.Should().NotBeNull();
            found!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailRegistered()
        {
            await CleanupAsync();
            var repository = new UserRepository(DbContext);
            var user = User.Register("existing@example.com", "hash", "User");
            await repository.AddAsync(user);
            await DbContext.SaveChangesAsync();

            var exists = await repository.ExistsByEmailAsync("existing@example.com");

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailNotRegistered()
        {
            await CleanupAsync();
            var repository = new UserRepository(DbContext);

            var exists = await repository.ExistsByEmailAsync("nonexistent@example.com");

            exists.Should().BeFalse();
        }

        [Fact]
        public async Task EmailUniqueContraint_ShouldPreventDuplicates()
        {
            await CleanupAsync();
            var repository = new UserRepository(DbContext);
            var user1 = User.Register("duplicate@example.com", "hash1", "User 1");
            var user2 = User.Register("duplicate@example.com", "hahs2", "User 2");

            await repository.AddAsync(user1);
            await DbContext.SaveChangesAsync();

            await repository.AddAsync(user2);
            var act = async () => await DbContext.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateException>();
        }
    }
}
