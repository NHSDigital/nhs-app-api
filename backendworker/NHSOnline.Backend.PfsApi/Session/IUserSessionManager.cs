using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface IUserSessionManager
    {
        Task<ProcessResult<UserSession, CreateSessionResult>> Create(
            CitizenIdSessionResult citizenIdSessionResult,
            ServiceJourneyRulesResponse serviceJourneyRules,
            string csrfToken);

        Task<DeleteUserSessionResult> Delete(
            HttpContext httpContext,
            UserSession userSession);
    }
}