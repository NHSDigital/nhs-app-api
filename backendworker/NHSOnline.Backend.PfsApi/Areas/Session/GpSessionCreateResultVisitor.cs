using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class GpSessionCreateResultVisitor : IGpSessionCreateResultVisitor<GpSessionCreateResultVisitorOutput>
    {
        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.SuccessfullyCreated successfullyCreated)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = true,
                Name = successfullyCreated.Name,
                UserSession = successfullyCreated.UserSession
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.InvalidIm1ConnectionToken invalidIm1ConnectionToken)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.InvalidUserCredentials invalidUserCredentials)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.InvalidRequest invalidRequest)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        
        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.SupplierSystemBadResponse supplierSystemBadResponse)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public GpSessionCreateResultVisitorOutput Visit(GpSessionCreateResult.UnknownError unknownError)
        {
            return new GpSessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}