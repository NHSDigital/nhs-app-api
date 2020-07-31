using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpUserSessionCreateResultVisitor : GpUserSessionCreateResultVisitorBase<GpUserSession>
    {
        private readonly ILogger _logger;
        private readonly Supplier _supplier;
        private readonly string _odsCode;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public GpUserSessionCreateResultVisitor(
            ILogger logger,
            Supplier supplier,
            string odsCode,
            IErrorReferenceGenerator errorReferenceGenerator) : base(logger, supplier)
        {
            _logger = logger;
            _supplier = supplier;
            _odsCode = odsCode;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        public override GpUserSession Visit(GpSessionCreateResult.Success result) => result.UserSession;

        protected override GpUserSession Failed(ErrorTypes userSessionError, string message)
        {
            _logger.LogWarning($"Creating gp session failed: {message}");
            _logger.LogInformation($"Creating null GP session for supplier={_supplier} odsCode={_odsCode}");

            var unavailableError = new ErrorTypes.GPSessionUnavailable(userSessionError);

            return new NullGpSession(
                _supplier, _errorReferenceGenerator.GenerateAndLogErrorReference(unavailableError));
        }
    }
}
