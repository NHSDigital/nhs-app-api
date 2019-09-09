using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.Support.UnitTests.Middleware
{
    [TestClass]
    public class ResponseHeaderMiddlewareTests
    {
        [TestMethod]
        public async Task Response_IsAdding_NoCacheHeaders()
        {
            // Arrange
            var feature = new DummyResponseFeature
            {
                Headers = new HeaderDictionary()
            };

            feature.Headers = new HeaderDictionary();
            var context = new DefaultHttpContext();

            context.Features.Set<IHttpResponseFeature>(feature);

            RequestDelegate next = async (ctx) => { await feature.InvokeCallBack(); };

            var subject = new ResponseHeaderMiddleware(next);

            // Act
            await subject.Invoke(context);

            // Assert
            var response = context.Response;
            response.Headers.Should().ContainKey(HeaderNames.CacheControl)
                .WhichValue.Equals("no-store, no-transform, must-revalidate, no-cache, private");

            response.Headers.Should().ContainKey(HeaderNames.Pragma)
                .WhichValue.Equals("no-cache");
        }
    }
}