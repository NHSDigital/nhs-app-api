using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionDeleteGpSessionStep: IUserSessionDeleteStep<P9UserSession>
    {
        private readonly IGpSessionManager _gpSessionManager;

        public UserSessionDeleteGpSessionStep(IGpSessionManager gpSessionManager)
            => _gpSessionManager = gpSessionManager;

        public async Task<bool> Delete(HttpContext httpContext, P9UserSession userSession)
        {
            var closeSessionResult = await _gpSessionManager.CloseSession(userSession.GpUserSession);

            return closeSessionResult.Accept(new CloseSessionResultVisitor());
        }

        private class CloseSessionResultVisitor : ICloseSessionResultVisitor<bool>
        {
            public bool Visit(CloseSessionResult.Success success) => true;

            public bool Visit(CloseSessionResult.Failure failure) => false;
        }
    }
}