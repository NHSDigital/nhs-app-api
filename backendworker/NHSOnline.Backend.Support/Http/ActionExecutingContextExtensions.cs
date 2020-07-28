using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Http
{
    public static class ActionExecutingContextExtensions
    {
        public static T GetRequiredService<T>(this ActionExecutingContext context)
        {
            return context.HttpContext
                .RequestServices
                .GetRequiredService<T>();
        }

        public static void EmitUnauthorisedResult<T>(this ActionExecutingContext context, string message)
        {
            context.GetRequiredService<ILogger<T>>()
                .LogInformation(message);

            context.Result = new UnauthorizedResult();
        }
    }
}
