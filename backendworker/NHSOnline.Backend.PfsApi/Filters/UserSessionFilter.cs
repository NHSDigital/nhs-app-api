using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "ASP.NET Filter")]
    public sealed class UserSessionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userSessionParameters = context.ActionDescriptor
                .Parameters
                .OfType<ControllerParameterDescriptor>()
                .Where(d => d.HasAttribute<UserSessionAttribute>());

            foreach (var userSessionParameter in userSessionParameters)
            {
                var session = context.GetRequiredService<IUserSessionService>()
                    .GetRequiredUserSession<UserSession>();
                var invalidParameterTypeDetected = !userSessionParameter.ParameterType
                    .IsInstanceOfType(session);

                if (invalidParameterTypeDetected)
                {
                    context.EmitUnauthorisedResult<UserSessionFilter>(
                        $"Action requires {userSessionParameter.ParameterType.Name} session but current " +
                        $"session is {session.GetType().Name}");

                    return;
                }

                context.ActionArguments[userSessionParameter.Name] = session;
            }

            await next();
        }
    }
}