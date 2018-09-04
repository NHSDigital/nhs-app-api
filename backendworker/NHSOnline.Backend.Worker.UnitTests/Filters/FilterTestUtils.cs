using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace NHSOnline.Backend.Worker.UnitTests.Filters
{
    public static class FilterTestUtils
    {
        internal static ActionExecutingContext CreateActionExecutingContext(FilterDescriptor filterDescriptor)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("name", "invalid");
            var filterDescriptionlist = new List<FilterDescriptor> { filterDescriptor };
            var actionDescription = new Mock<ActionDescriptor>().Object;
            actionDescription.FilterDescriptors = filterDescriptionlist;
            var actionContext = new ActionContext(
                new Mock<HttpContext>().Object,
                new Mock<RouteData>().Object,
                actionDescription,
                modelState
            );
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object
            );
            return actionExecutingContext;
        }
    }
}