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

            if (gpLinkedAccountModel.PatientId == tppUserSession.Id)
            {
                tppRequestParameters = new TppRequestParameters(tppUserSession);
            }
            else if (tppUserSession.HasLinkedAccounts)
            {
                var proxy = tppUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId);

                if (proxy != null)
                {
                    tppRequestParameters = new TppRequestParameters(tppUserSession)
                    {
                        PatientId = proxy.PatientId,
                        Suid = proxy.Suid,
                    };
                }
            }

            if (tppRequestParameters == null)
            {
                logger.LogInformation($"Patient Id {gpLinkedAccountModel.PatientId} not matched. " +
                                      $"Main user id {tppUserSession.Id} and user had linkedAccounts: {tppUserSession.HasLinkedAccounts}");

                throw new InvalidPatientIdException(gpLinkedAccountModel.PatientId);
            }

            return tppRequestParameters;
        }
    }
}