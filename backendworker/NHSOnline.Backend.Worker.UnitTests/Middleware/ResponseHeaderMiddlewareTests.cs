using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Worker.UnitTests.Middleware
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
            Assert.IsTrue(response.Headers.TryGetValue("Cache-Control", out var headerValues));
            string actualHeaders = string.Join(", ", headerValues);
            Assert.IsTrue("no-store, no-transform, no-cache, private".Equals(actualHeaders, StringComparison.Ordinal));
        }
    }
}