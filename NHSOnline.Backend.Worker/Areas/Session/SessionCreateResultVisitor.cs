using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class
        SessionCreateResultVisitor : ISessionCreateResultVisitor<SessionCreateResultVisitorOutput>
    {
        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.SuccessfullyCreated result)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = true,
                FamilyName = result.FamilyName,
                GivenName = result.GivenName,
                UserSession = result.UserSession
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.InvalidIm1ConnectionToken result)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.SupplierSystemUnavailable result)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}