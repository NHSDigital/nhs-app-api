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

            if (gpLinkedAccountModel.PatientId == emisUserSession.Id)
            {
                userPatientLinkToken = emisUserSession.UserPatientLinkToken;
                logger.LogInformation("Patient Id match found against main (logged in) user");
            }
            else if (emisUserSession.HasLinkedAccounts)
            {
                userPatientLinkToken = emisUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId)?.UserPatientLinkToken;

                if (userPatientLinkToken != null)
                {
                    logger.LogInformation("Patient Id match found on proxy patient");
                }
            }

            if (string.IsNullOrEmpty(userPatientLinkToken))
            {
                throw new InvalidPatientIdException(gpLinkedAccountModel.PatientId);
            }

            return new EmisRequestParameters(emisUserSession)
            {
                UserPatientLinkToken = userPatientLinkToken,
            };
        }
    }
}