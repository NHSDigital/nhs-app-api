using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientPatientOverviewPost : ITppClientRequest<TppUserSession, ViewPatientOverviewReply>
    {
        private readonly ILogger<TppClientPatientOverviewPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientPatientOverviewPost(ILogger<TppClientPatientOverviewPost> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<ViewPatientOverviewReply>> Post(TppUserSession tppUserSession)
        {
            _logger.LogEnter();

            try
            {
                var request = new ViewPatientOverview
                {
                    PatientId = tppUserSession.PatientId,
                    OnlineUserId = tppUserSession.OnlineUserId,
                    UnitId = tppUserSession.OdsCode,
                };

                return await _requestExecutor.Post<ViewPatientOverviewReply>(
                    requestBuilder => requestBuilder.Model(request).Suid(tppUserSession.Suid));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}