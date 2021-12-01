using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
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
            _p9UserSession.PatientSessionId = result.UserSession.Id;
            _p9UserSession.PatientLookup ??= new Dictionary<Guid, string>();

            // The following checks are temporary be replaced as part of jira 13005
            if (result.UserSession is EmisUserSession emisUserSession)
            {
                if (!_p9UserSession.PatientLookup.ContainsValue(emisUserSession.PatientActivityContextGuid))
                {
                    _p9UserSession.PatientLookup.Add(_p9UserSession.PatientSessionId, emisUserSession.PatientActivityContextGuid);
                }

                foreach (var emisProxyUserSession in emisUserSession.ProxyPatients)
                {
                    // ID will be created here.
                    // Using EmisProxyUserSession.Id for now for backwards compatibility.
                    _p9UserSession.PatientLookup.Add(
                        emisProxyUserSession.Id,
                        emisProxyUserSession.PatientActivityContextGuid);
                }
            }
            else if (result.UserSession is TppUserSession tppUserSession)
            {
                if (!_p9UserSession.PatientLookup.ContainsValue(tppUserSession.PatientId))
                {
                    _p9UserSession.PatientLookup.Add(_p9UserSession.PatientSessionId, tppUserSession.PatientId);
                }

                foreach (var tppProxyUserSession in tppUserSession.ProxyPatients)
                {
                    // ID will be created here.
                    // Using TppProxyUserSession.Id for now for backwards compatibility.
                    _p9UserSession.PatientLookup.Add(
                        tppProxyUserSession.Id,
                        tppProxyUserSession.PatientId);
                }
            }
            else
            {
                // Other providers just have to have the main PatientSessionId in the dictionary.
                // The actual value doesn't matter as there is no proxy potential.
                if (!_p9UserSession.PatientLookup.ContainsKey(_p9UserSession.PatientSessionId))
                {
                    _p9UserSession.PatientLookup.Add(_p9UserSession.PatientSessionId, string.Empty);
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
