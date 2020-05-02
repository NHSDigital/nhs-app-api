using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionDeleteSignOutStep : IUserSessionDeleteStep<UserSession>
    {
        public async Task<bool> Delete(HttpContext httpContext, UserSession userSession)
        {
            // TODO: this should be alongside the call to SignInAsync
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }
    }
}