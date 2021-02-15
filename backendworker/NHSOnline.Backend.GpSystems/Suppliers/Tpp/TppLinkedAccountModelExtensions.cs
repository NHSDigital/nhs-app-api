using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class TppLinkedAccountModelExtensions
    {
        public static TppRequestParameters BuildTppRequestParameters(this GpLinkedAccountModel gpLinkedAccountModel, ILogger logger)
        {
            TppRequestParameters tppRequestParameters = null;
            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

            if (tppUserSession.GetCurrentlyAuthenticatedId() != gpLinkedAccountModel.RequestingPatientGpIdentifier)
            {
                throw new InvalidPatientIdException(
                    "Request rejected as user currently not authenticated with TPP");
            }

            if (gpLinkedAccountModel.RequestingPatientGpIdentifier == tppUserSession.PatientId)
            {
                tppRequestParameters = new TppRequestParameters(tppUserSession);
            }
            else if (tppUserSession.HasLinkedAccounts)
            {
                var proxy = tppUserSession.ProxyPatients
                    .FirstOrDefault(x => x.PatientId == gpLinkedAccountModel.RequestingPatientGpIdentifier);

                if (proxy != null)
                {
                    tppRequestParameters = new TppRequestParameters(tppUserSession, proxy);
                }
            }

            if (tppRequestParameters == null)
            {
                logger.LogInformation("Patient Id not matched. " +
                                      $"Main user had linkedAccounts: {tppUserSession.HasLinkedAccounts}");

                throw new InvalidPatientIdException();
            }

            return tppRequestParameters;
        }
    }
}