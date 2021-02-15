using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public static class EmisLinkedAccountModelExtensions
    {
        public static EmisRequestParameters BuildEmisRequestParameters(this GpLinkedAccountModel gpLinkedAccountModel, ILogger logger)
        {
            string userPatientLinkToken = null;
            var emisUserSession = (EmisUserSession) gpLinkedAccountModel.GpUserSession;

            if (gpLinkedAccountModel.RequestingPatientGpIdentifier == emisUserSession.PatientActivityContextGuid)
            {
                userPatientLinkToken = emisUserSession.UserPatientLinkToken;
            }
            else if (emisUserSession.HasLinkedAccounts)
            {
                // just double check is valid before using
                var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x =>
                    x.PatientActivityContextGuid == gpLinkedAccountModel.RequestingPatientGpIdentifier);

                if (proxy != null)
                {
                    userPatientLinkToken = proxy.UserPatientLinkToken;
                }
            }

            if (string.IsNullOrEmpty(userPatientLinkToken))
            {
                logger.LogInformation("Patient Id not matched.");
                throw new InvalidPatientIdException();
            }

            return new EmisRequestParameters(emisUserSession)
            {
                UserPatientLinkToken = userPatientLinkToken,
            };
        }
    }
}