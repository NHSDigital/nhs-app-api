using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class GpSessionCreateResultVisitor : IGpSessionCreateResultVisitor<GpSessionCreateResultVisitorOutput>
    {
        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.Success result)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = true,
                Name = result.Name,
                UserSession = result.UserSession
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.Forbidden result)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.BadGateway result)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.InternalServerError result)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.BadRequest result)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}