using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.ApiSupport.UnitTests.Middleware
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

            Assert.IsTrue(response.Headers.TryGetValue(HeaderNames.CacheControl, out var cacheControlHeaderValues));
            Assert.AreEqual("no-store, no-transform, must-revalidate, no-cache, private", cacheControlHeaderValues.ToString());

            Assert.IsTrue(response.Headers.TryGetValue(HeaderNames.Pragma, out var pragmaHeaderValues));
            Assert.AreEqual("no-cache", pragmaHeaderValues.ToString());
        }
    }
}