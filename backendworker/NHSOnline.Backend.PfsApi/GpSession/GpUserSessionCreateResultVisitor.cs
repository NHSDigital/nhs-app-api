using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpUserSessionCreateResultVisitor : GpUserSessionCreateResultVisitorBase<Task>
    {
        private readonly ILogger _logger;
        private readonly Supplier _supplier;
        private readonly P9UserSession _p9UserSession;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public GpUserSessionCreateResultVisitor(
            ILogger logger,
            Supplier supplier,
            P9UserSession p9UserSession,
            IErrorReferenceGenerator errorReferenceGenerator) : base(logger, supplier)
        {
            _logger = logger;
            _supplier = supplier;
            _p9UserSession = p9UserSession;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        public override Task Visit(GpSessionCreateResult.Success result)
        {
            _p9UserSession.GpUserSession = result.UserSession;

            if (!_p9UserSession.PatientLookup.ContainsValue(result.MainPatientGpIdentifier))
            {
                _p9UserSession.PatientLookup.Add(_p9UserSession.PatientSessionId, result.MainPatientGpIdentifier);
            }

            foreach (var patientIdentifier in result.ProxyPatientGpIdentifiers)
            {
                if (!_p9UserSession.PatientLookup.ContainsValue(patientIdentifier))
                {
                    _p9UserSession.PatientLookup.Add(Guid.NewGuid(), patientIdentifier);
                }
            }

            return Task.CompletedTask;
        }

        protected override Task Failed(ErrorTypes userSessionError, string message)
        {
            _logger.LogWarning($"Creating gp session failed: {message}");
            _logger.LogInformation($"Creating null GP session for supplier={_supplier} odsCode={_p9UserSession.OdsCode}");

            var unavailableError = new ErrorTypes.GPSessionUnavailable(userSessionError);

            var nullGpSession = new NullGpSession(
                _supplier, _errorReferenceGenerator.GenerateAndLogErrorReference(unavailableError));

            _p9UserSession.GpUserSession = nullGpSession;

            return Task.CompletedTask;
        }
    }
}