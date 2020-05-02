using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal interface IUserSessionDeleteStep<in TUserSession> where TUserSession: UserSession
    {
        Task<bool> Delete(HttpContext httpContext, TUserSession userSession);
    }
}