using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpSessionCreateResultVisitor : IGpSessionCreateResultVisitor<GpUserSession>
    {
        private readonly ILogger _logger;
        private readonly string _odsCode;
        private readonly Supplier _supplier;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public GpSessionCreateResultVisitor(
            ILogger logger,
            string odsCode,
            Supplier supplier,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            _logger = logger;
            _odsCode = odsCode;
            _supplier = supplier;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        public GpUserSession Visit(GpSessionCreateResult.Success result)
            => result.UserSession;

        public GpUserSession Visit(GpSessionCreateResult.Forbidden result)
            => Failed(new ErrorTypes.LoginForbidden(), result.Message);

        public GpUserSession Visit(GpSessionCreateResult.BadGateway result)
        => Failed(ErrorTypes.LoginBadGateway(_logger, _supplier), result.Message);

        public GpUserSession Visit(GpSessionCreateResult.ErrorExceptionResult result)
        {
            return Failed(new ErrorTypes.LoginErrorExceptionResult(), result.ErrorMessage);
        }

        public GpUserSession Visit(GpSessionCreateResult.Timeout result)
            => Failed(ErrorTypes.LoginTimeout(_logger, _supplier), result.ErrorMessage);

        public GpUserSession Visit(GpSessionCreateResult.Unparseable result)
            => Failed(new ErrorTypes.LoginGPUnparseable(), result.ErrorMessage);

        public GpUserSession Visit(GpSessionCreateResult.InternalServerError result)
            => Failed(new ErrorTypes.LoginUnexpectedError(), result.Message);

        public GpUserSession Visit(GpSessionCreateResult.BadRequest result)
            => Failed(new ErrorTypes.LoginBadRequest(), result.Message);

        public GpUserSession Visit(GpSessionCreateResult.InvalidConnectionToken result)
            => Failed(new ErrorTypes.LoginForbidden(), "Invalid connection token");

        private GpUserSession Failed(ErrorTypes errorType, string message)
        {
            _logger.LogWarning($"Creating the session failed: {message}");
            _logger.LogInformation($"Creating null GP session for supplier={_supplier} odsCode={_odsCode}");

            var unavailableError = new ErrorTypes.GPSessionUnavailable(errorType);

            return new NullGpSession(
                _supplier, _errorReferenceGenerator.GenerateAndLogErrorReference(unavailableError));
        }
    }
}
