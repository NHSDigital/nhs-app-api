using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Prescription;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisPrescriptionService : IPrescriptionService
    {
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;
        private readonly ILogger _logger;

        public EmisPrescriptionService(ILoggerFactory loggerFactory, IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _logger = loggerFactory.CreateLogger<EmisPrescriptionService>();
        }

        public async Task<GetPrescriptionsResult> Get(UserSession userSession, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, fromDate, toDate);

                if (!prescriptionsResponse.HasSuccessStatusCode)
                {
                    _logger.LogError($"Unsuccessful request retrieving prescriptions for {nameof(fromDate)}={fromDate:O}, {nameof(toDate)}={toDate:O}");
                    return new GetPrescriptionsResult.Unsuccessful();
                }

                _logger.LogDebug($"Mapping response from {nameof(PrescriptionRequestsGetResponse)} to {nameof(PrescriptionListResponse)}");
                var result = _emisPrescriptionMapper.Map(prescriptionsResponse.Body);

                return new GetPrescriptionsResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new GetPrescriptionsResult.Unsuccessful();
            }
        }
    }
}
