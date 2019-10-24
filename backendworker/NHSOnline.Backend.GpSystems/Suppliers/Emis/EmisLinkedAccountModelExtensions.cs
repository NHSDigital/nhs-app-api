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
            else if (emisUserSession.ProxyPatients != null && emisUserSession.ProxyPatients.Any())
            {
                userPatientLinkToken = emisUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId)?.UserPatientLinkToken;
                
                if (userPatientLinkToken != null)
                {
                    logger.LogInformation("Patient Id match found on proxy patient");
                }
            }
            else
            {
                logger.LogInformation("No Proxy Patients in EmisUserSession");
            }

            // For now, default to the main (logged in user) to prevent any potential errors in production.
            // A Future story will be created for error handling when all applicable endpoints are using the
            // FromHeader Attribute to set the ProfileGUID received from the client.
            if (string.IsNullOrEmpty(userPatientLinkToken))
            {
                var msg = $"Could not find a matching Id in EmisUserSession for " +
                          $"{gpLinkedAccountModel.PatientId}, defaulting to UserPatientLinkToken of main (logged in) user";
                logger.LogError(msg);

                userPatientLinkToken = emisUserSession.UserPatientLinkToken;
            }

            return new EmisRequestParameters(emisUserSession, userPatientLinkToken);
        }
    }
}