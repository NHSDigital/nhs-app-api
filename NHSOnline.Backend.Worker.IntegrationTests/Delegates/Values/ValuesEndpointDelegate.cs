using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace NHSOnline.Backend.Worker.IntegrationTests.Delegates.Values
{
    public static class ValuesEndpointDelegate
    {
        public static void RespondsWithCorrectValues(FluentMockServer stubServer)
        {
            stubServer.Given(
                    Request.Create()
                        .WithPath("/api/values")
                        .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("[\"Stubbed Value 1\",\"Stubbed Value 2\"]"));
        }
    }
}
