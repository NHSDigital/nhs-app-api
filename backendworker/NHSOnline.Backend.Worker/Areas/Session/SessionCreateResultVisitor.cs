using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionCreateResultVisitor : ISessionCreateResultVisitor<SessionCreateResultVisitorOutput>
    {
        private readonly IOptions<ConfigurationSettings> _settings;

        public SessionCreateResultVisitor(IOptions<ConfigurationSettings> settings)
        {
            _settings = settings;
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.SuccessfullyCreated successfullyCreated)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = true,
                Name = successfullyCreated.Name,
                UserSession = successfullyCreated.UserSession,
                SessionTimeout = _settings.Value.DefaultSessionExpiryMinutes * 60
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.InvalidIm1ConnectionToken invalidIm1ConnectionToken)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.InvalidUserCredentials invalidUserCredentials)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.InvalidRequest invalidRequest)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        
        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.SupplierSystemBadResponse supplierSystemBadResponse)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public SessionCreateResultVisitorOutput Visit(SessionCreateResult.UnknownError unknownError)
        {
            return new SessionCreateResultVisitorOutput
            {
                SessionWasCreated = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}