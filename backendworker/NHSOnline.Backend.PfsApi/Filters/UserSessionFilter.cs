using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.PfsApi.Filters
{
    public sealed class UserSessionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var userSessionParameter in context.ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>().Where(HasUserSessionAttribute))
            {
                var session = context.HttpContext.GetUserSession();
                if (userSessionParameter.ParameterType.IsInstanceOfType(session))
                {
                    context.ActionArguments[userSessionParameter.Name] = session;
                }
                else
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UserSessionFilter>>();
                    logger.LogInformation(
                        $"Action requires {userSessionParameter.ParameterType.Name} session but current session is {session?.GetType().Name ?? "null"}");
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }

            await next();
        }

        private bool HasUserSessionAttribute(ControllerParameterDescriptor descriptor)
            => descriptor.ParameterInfo.CustomAttributes.Any(HasUserSessionAttribute);

        private static bool HasUserSessionAttribute(CustomAttributeData attribute)
            => attribute.AttributeType == typeof(UserSessionAttribute);
    }
}