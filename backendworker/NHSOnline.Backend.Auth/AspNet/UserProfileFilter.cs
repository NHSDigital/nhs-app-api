using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.AspNet
{
    public class UserProfileFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UserProfileFilter>>();

            foreach (var userProfileParameter in context.ActionDescriptor.Parameters
                .OfType<ControllerParameterDescriptor>().Where(HasUserProfileAttribute))
            {
                var userProfileService = context.HttpContext.RequestServices.GetRequiredService<UserProfileService>();
                var userProfileOption = await userProfileService.GetUserProfile();

                if (!userProfileOption.HasValue)
                {
                    logger.LogError(
                        $"Action requires {typeof(UserProfile)} which could not be retrieved");
                    context.Result = new StatusCodeResult(StatusCodes.Status502BadGateway);
                    return;
                }

                var userProfile = userProfileOption.ValueOrFailure();
                userProfileService.SetUserProfile(userProfile);
                if (!userProfileParameter.ParameterType.IsInstanceOfType(userProfile))
                {
                    logger.LogInformation(
                        $"Action requires {userProfileParameter.ParameterType.Name} but user profile found was of type {userProfile.GetType().Name}");
                    context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    return;
                }

                context.ActionArguments[userProfileParameter.Name] = userProfile;
            }

            await next();
        }

        private bool HasUserProfileAttribute(ControllerParameterDescriptor descriptor)
            => descriptor.ParameterInfo.CustomAttributes.Any(HasUserProfileAttribute);

        private static bool HasUserProfileAttribute(CustomAttributeData attribute)
            => attribute.AttributeType == typeof(UserProfileAttribute);
    }
}