using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface IUserSessionManager
    {
        Task<CreateUserSessionResult> Create(
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken);

        Task<bool> SignOutAsync(HttpContext httpContext);
    }
}