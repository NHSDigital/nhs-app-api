using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class SessionExtendResultVisitor : ISessionExtendResultVisitor<SessionExtendResultVisitorOutput>
    {
        public SessionExtendResultVisitorOutput Visit(SessionExtendResult.Success result)
        {
            return new SessionExtendResultVisitorOutput
            {
                SessionWasExtended = true,
                StatusCode = StatusCodes.Status200OK
            };
        }

        public SessionExtendResultVisitorOutput Visit(SessionExtendResult.BadGateway result)
        {
            return new SessionExtendResultVisitorOutput
            {
                SessionWasExtended = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public SessionExtendResultVisitorOutput Visit(SessionExtendResult.InternalServerError result)
        {
            return new SessionExtendResultVisitorOutput
            {
                SessionWasExtended = false,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}