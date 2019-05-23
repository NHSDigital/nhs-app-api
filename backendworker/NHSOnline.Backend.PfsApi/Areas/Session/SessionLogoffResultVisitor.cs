using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class SessionLogoffResultVisitor : ISessionLogoffResultVisitor<SessionLogoffResultVisitorOutput>
    {
        public SessionLogoffResultVisitorOutput Visit(SessionLogoffResult.Success result)
        {
            return new SessionLogoffResultVisitorOutput
            {
                SessionWasDeleted = true,
                GpUserSession = result.GpUserSession,
            };
        } 

        public SessionLogoffResultVisitorOutput Visit(SessionLogoffResult.Forbidden result)
        {
            return new SessionLogoffResultVisitorOutput
            {
                SessionWasDeleted = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public SessionLogoffResultVisitorOutput Visit(SessionLogoffResult.BadGateway result)
        {
            return new SessionLogoffResultVisitorOutput
            {
                SessionWasDeleted = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}