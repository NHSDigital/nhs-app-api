using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionExtendResultVisitor : ISessionExtendResultVisitor<SessionExtendResultVisitorOutput>
    {
        public SessionExtendResultVisitorOutput Visit(SessionExtendResult.SuccessfullyExtended successfullyExtended)
        {
            return new SessionExtendResultVisitorOutput
            {
                SessionWasExtended = true,
                StatusCode = StatusCodes.Status200OK
            };
        }

        public SessionExtendResultVisitorOutput Visit(SessionExtendResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            return new SessionExtendResultVisitorOutput
            {
                SessionWasExtended = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}