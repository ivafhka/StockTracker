using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace StockTracker.ArchTests
{
    public class LayerDependencyTests
    {
        private const string DomainNamespace = "StockTracker.Domain";
        private const string ApplicationNamespace = "StockTracker.Application";
        private const string InfrastructureNamespace = "StockTracker.Infrastructure";
        private const string ApiNamespace = "StockTracker.Api";

        private static readonly Assembly DomainAssembly =
            Assembly.Load("StockTracker.Domain");

        private static readonly Assembly ApplicationAssembly =
            Assembly.Load("StockTracker.Application");
        private static readonly Assembly InfrastructureAssembly =
            Assembly.Load("StockTracker.Infrastructure");

        [Fact]
        public void Domain_ShouldNotDependOn_Application()
        {
            var result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOn(ApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Domain should not depend on Application. Failing types: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Domain_ShouldNotDependOn_Infrastructure()
        {
            var result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOn(InfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Domain should not depend on Infrastructure. Failing types: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Domain_ShouldNotDependOn_Api()
        {
            var result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOn(ApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Domain should not depend on Api. Failing types: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Application_ShouldNotDependOn_Infrastructure()
        {
            var result = Types.InAssembly(ApplicationAssembly)
                .Should()
                .NotHaveDependencyOn(InfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Application should not depend on Infrastructure. Failing types: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Application_ShouldNotDependOn_Api()
        {
            var result = Types.InAssembly(ApplicationAssembly)
                .Should()
                .NotHaveDependencyOn(ApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Application should not depend on Api. Failing types: {GetFailingTypes(result)}");
        }

        [Fact]
        public void Infrastructure_ShouldNotDependOn_Api()
        {
            var result = Types.InAssembly(InfrastructureAssembly)
                .Should()
                .NotHaveDependencyOn(ApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Infrastructure should not depend on Api. Failing types: {GetFailingTypes(result)}");
        }


        private static string GetFailingTypes(TestResult result)
        {
            if (result.FailingTypes is null) return "none";
            return string.Join(",", result.FailingTypes.Select(t => t.FullName));
        }
    }
}
