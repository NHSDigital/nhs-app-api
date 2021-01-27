using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class TestMethodExtensions
    {
        internal static bool HasInvalidParameters<TDriverWrapper>(this ITestMethod testMethod, [NotNullWhen(true)] out TestResult? errorResult)
        {
            if (testMethod.ParameterTypes.Length != 1 ||
                !testMethod.ParameterTypes[0].ParameterType.IsAssignableFrom(typeof(TDriverWrapper)))
            {
                var message = $"Test method must take a single argument of type {typeof(TDriverWrapper).Name}";
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