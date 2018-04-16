using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Filters;

namespace NHSOnline.Backend.Worker.UnitTests.Filters
{
    [TestClass]
    public class CorsAuthorityFilterTests
    {
        [TestMethod]
        public void OnActionExecuting_SetsTheAccessControlAllowOriginHeader_WhenThereIsACorsAuthorityEnvironmentVariable()
        {
            const string expected = "*";
            System.Environment.SetEnvironmentVariable("CORS_AUTHORITY", expected);
            var httpContext = MockHttpContext();
            var actionContext = CreateActionContext(httpContext.Object);
            var context = CreateActionExecutingContext(actionContext);
            var filter  = new AccessControlAllowOriginFilter();
            filter.OnActionExecuting(context);

            actionContext.HttpContext.Response.Headers.Should().Contain(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", expected));
        }

        [TestMethod]
        public void
            OnActionExecuting_DoesNotSetTheAccessControlAllowOriginHeader_WhenThereIsNoCorsAuthorityEnvironmentVariable()
        {
            var httpContext = MockHttpContext();
            var actionContext = CreateActionContext(httpContext.Object);
            var context = CreateActionExecutingContext(actionContext);
            var filter = new AccessControlAllowOriginFilter();
            filter.OnActionExecuting(context);

            actionContext.HttpContext.Response.Headers.Should().NotContainKey("Access-Control-Allow-Origin");
        }

        private Mock<HttpContext> MockHttpContext()
        {
            var httpContext = new Mock<HttpContext>();
            var httpResponse = new Mock<HttpResponse>();

            httpContext.SetupGet(x => x.Response).Returns(httpResponse.Object);
            httpResponse.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            return httpContext;
        }

        private ActionContext CreateActionContext(HttpContext httpContext)
        {
            return new ActionContext(
                httpContext,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object,
                new ModelStateDictionary()
            );
        }

        private ActionExecutingContext CreateActionExecutingContext(ActionContext actionContext)
        {
            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );
        }
    }
}
