using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace StockTracker.ArchTests
{
    public class NamingConventionTests
    {
        private static readonly Assembly DomainAssembly =
            typeof(StockTracker.Domain.Common.Entity).Assembly;

        [Fact]
        public void Interfaces_ShouldstartWith_LetterI()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Interfaces must start with 'I'. Failing types: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Repositories_ShouldBeInterfaces_InDomainLayer()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That().
                HaveNameEndingWith("Repository")
                .Should()
                .BeInterfaces()
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"In Domain layer, all 'Repository' types must be interfaces. Failing: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Entities_ShouldBeInIntitiesNamespace()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .Inherit(typeof(StockTracker.Domain.Common.Entity))
                .Should()
                .ResideInNamespace("StockTracker.Domain.Entities")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Entities must live in Entities namespace. Failing: {GetFailingTypes(result)}");
        }

        [Fact]
        public void ValueObject_ShouldBeInValueObjectsNamespace()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .Inherit(typeof(StockTracker.Domain.Common.ValueObject))
                .Should()
                .ResideInNamespace("StockTracker.Domain.ValueObjects")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Value Object must live in ValueObjects namespace. Failing: {GetFailingTypes(result)}");
        }

        [Fact]
        public void DomainEvents_ShouldHaveEventSuffix()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .ImplementInterface(typeof(StockTracker.Domain.Common.IDomainEvents))
                .Should()
                .HaveNameEndingWith("Event")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Domain events must end with 'Event'. Failing: {GetFailingTypes(result)}");
        }

        private static string GetFailingTypes(TestResult result)
        {
            if (result.FailingTypes is null) return "none";
            return string.Join(",", result.FailingTypes.Select(t => t.FullName));
        }
    }
}
