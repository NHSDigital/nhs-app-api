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
            }
            else if (emisUserSession.HasLinkedAccounts)
            {
                userPatientLinkToken = emisUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId)?.UserPatientLinkToken;
            }

            if (string.IsNullOrEmpty(userPatientLinkToken))
            {
                logger.LogInformation($"Patient Id {gpLinkedAccountModel.PatientId} not matched. Main user id {emisUserSession.Id} and user had linkedAccounts: {emisUserSession.HasLinkedAccounts}");
                throw new InvalidPatientIdException(gpLinkedAccountModel.PatientId);
            }

            return new EmisRequestParameters(emisUserSession)
            {
                UserPatientLinkToken = userPatientLinkToken,
            };
        }
    }
}