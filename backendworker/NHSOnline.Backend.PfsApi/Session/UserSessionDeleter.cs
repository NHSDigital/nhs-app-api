using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionDeleter
    {
        private readonly IAuditor _auditor;
        private readonly UserSessionDeleteSteps _userSessionDeleteSteps;

        public UserSessionDeleter(
            IAuditor auditor,
            UserSessionDeleteSteps userSessionDeleteSteps)
        {
            _auditor = auditor;
            _userSessionDeleteSteps = userSessionDeleteSteps;
        }

        public async Task<DeleteUserSessionResult> Delete(HttpContext httpContext, UserSession userSession)
            => await userSession.Accept(new DeleteUserSessionVisitor(this, httpContext));

        private async Task<DeleteUserSessionResult> Delete(HttpContext httpContext, P5UserSession userSession)
            => await _userSessionDeleteSteps.Delete(httpContext, userSession);

        private async Task<DeleteUserSessionResult> Delete(HttpContext httpContext, P9UserSession userSession)
        {
            var gpUserSession = userSession.GpUserSession;
            var citizenIdUserSession = userSession.CitizenIdUserSession;

            return await _auditor
                .Audit()
                .AccessToken(citizenIdUserSession.AccessToken)
                .NhsNumber(gpUserSession.NhsNumber)
                .Supplier(gpUserSession.Supplier)
                .Operation(AuditingOperations.SessionDelete)
                .Details("Session delete called")
                .Execute(async () => await _userSessionDeleteSteps.Delete(httpContext, userSession));
        }

        private sealed class DeleteUserSessionVisitor : IUserSessionVisitor<Task<DeleteUserSessionResult>>
        {
            private readonly HttpContext _httpContext;
            private readonly UserSessionDeleter _userSessionDeleter;

            public DeleteUserSessionVisitor(UserSessionDeleter userSessionDeleter, HttpContext httpContext)
            {
                _userSessionDeleter = userSessionDeleter;
                _httpContext = httpContext;
            }

            public async Task<DeleteUserSessionResult> Visit(P5UserSession userSession)
                => await _userSessionDeleter.Delete(_httpContext, userSession);

            public async Task<DeleteUserSessionResult> Visit(P9UserSession userSession)
                => await _userSessionDeleter.Delete(_httpContext, userSession);
        }
    }
}