using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Worker.GpSystems.Session;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionLogoffResultVisitor : ISessionLogoffResultVisitor<SessionLogoffResultVisitorOutput>
    {
        public SessionLogoffResultVisitorOutput Visit(SessionLogoffResult.SuccessfullyDeleted successfullyDeleted)
        {
            return new SessionLogoffResultVisitorOutput
            {
                SessionWasDeleted = true,
                UserSession = successfullyDeleted.UserSession,
            };
        } 

        public SessionLogoffResultVisitorOutput Visit(SessionLogoffResult.NotAuthenticated notAuthenticated)
        {
            return new SessionLogoffResultVisitorOutput
            {
                SessionWasDeleted = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public SessionLogoffResultVisitorOutput Visit(SessionLogoffResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            return new SessionLogoffResultVisitorOutput
            {
                SessionWasDeleted = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}