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
                ShouldReturn = false,
                UserSessionResponse = result.UserInfo,
                SupplierSessionId = result.SessionId
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.InvalidIm1ConnectionToken result)
        {
            return new SessionCreateResultVisitorOutput
            {
                ShouldReturn = true,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.SupplierSystemUnavailable result)
        {
            return new SessionCreateResultVisitorOutput
            {
                ShouldReturn = true,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}
