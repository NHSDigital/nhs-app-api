using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionManager : IUserSessionManager
    {
        private readonly UserSessionCreator _userSessionCreator;
        private readonly UserSessionDeleter _userSessionDeleter;

        public UserSessionManager(
            UserSessionCreator userSessionCreator,
            UserSessionDeleter userSessionDeleter)
        {
            _userSessionCreator = userSessionCreator;
            _userSessionDeleter = userSessionDeleter;
        }

        public async Task<ProcessResult<UserSession, CreateSessionResult>> Create(
            CitizenIdSessionResult citizenIdSessionResult,
            ServiceJourneyRulesResponse serviceJourneyRules,
            string csrfToken)
        {
            var createUserSessionResult = await _userSessionCreator.Create(citizenIdSessionResult, serviceJourneyRules, csrfToken);

            return createUserSessionResult.Accept(
                failure => ProcessResult.FinalResult<UserSession, CreateSessionResult>(new CreateSessionResult.Error(failure.ErrorType)),
                success => ProcessResult.StepResult<UserSession, CreateSessionResult>(success.UserSession),
                onSuccessNoGpSession => ProcessResult.StepResult<UserSession, CreateSessionResult>(onSuccessNoGpSession.UserSession));
        }

        public async Task<DeleteUserSessionResult> Delete(HttpContext httpContext, UserSession userSession)
        {
            return await _userSessionDeleter.Delete(httpContext, userSession);
        }
    }
}
