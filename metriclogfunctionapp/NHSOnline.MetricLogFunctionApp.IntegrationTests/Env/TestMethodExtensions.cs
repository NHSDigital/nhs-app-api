using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env
{
    internal static class TestMethodExtensions
    {
        internal static bool HasInvalidParameters<TSourceWrapper>(this ITestMethod testMethod, [NotNullWhen(true)] out TestResult errorResult)
        {
            if (testMethod.ParameterTypes.Length != 2 ||
                !testMethod.ParameterTypes[0].ParameterType.IsAssignableFrom(typeof(TSourceWrapper)) ||
                !testMethod.ParameterTypes[1].ParameterType.IsAssignableFrom(typeof(TestEnv)))
            {
                var message = $"Test method must take two arguments of type {typeof(TSourceWrapper).Name} and {nameof(TestEnv)}";
                errorResult = new TestResult
                {
                    DisplayName = testMethod.TestMethodName,
                    Outcome = UnitTestOutcome.NotRunnable,
                    TestFailureException = new AssertFailedException(message)
                };
                return true;
            }

            errorResult = null;
            return false;
        }

        internal static bool HasInvalidParameters(this ITestMethod testMethod, [NotNullWhen(true)] out TestResult errorResult)
        {
            if (testMethod.ParameterTypes.Length != 1 ||
                !testMethod.ParameterTypes[0].ParameterType.IsAssignableFrom(typeof(TestEnv)))
            {
                var message = $"Test method must take one argument of type {nameof(TestEnv)}";
                errorResult = new TestResult
                {
                    DisplayName = testMethod.TestMethodName,
                    Outcome = UnitTestOutcome.NotRunnable,
                    TestFailureException = new AssertFailedException(message)
                };
                return true;
            }

            errorResult = null;
            return false;
        }
    }
}