using System.Linq;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nhs.App.Api.Integration.Tests.Extensions
{
    internal static class HttpResponseHeadersExtensions
    {
        internal static void ShouldContainHeader(this HttpResponseHeaders headers, string expectedName, string expectedValue)
        {
            if (!headers.TryGetValues(expectedName, out var values))
            {
                Assert.Fail($"Expected HTTP header present with name '{expectedName}', but none was found.");
            }

            var valuesArray = values.ToArray();

            if (valuesArray.Length!=1)
            {
                Assert.Fail($"Expected HTTP header with name '{expectedName}' to contain a single value, but found {valuesArray.Length}.");
            }

            var actualValue = valuesArray.Single();
            if (actualValue != expectedValue)
            {
                Assert.Fail($"Expected HTTP header with name '{expectedName}' to have value '{expectedValue}', but found '{actualValue}'.");
            }
        }

        internal static void ShouldNotContainHeader(this HttpResponseHeaders headers, string headerName)
        {
            if (headers.TryGetValues(headerName, out var _))
            {
                Assert.Fail($"Expected no HTTP header present with name '{headerName}', but one was found.");
            }
        }
    }
}
