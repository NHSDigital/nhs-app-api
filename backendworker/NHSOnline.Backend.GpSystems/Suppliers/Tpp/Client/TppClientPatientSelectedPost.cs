using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientPatientSelectedPost : ITppClientRequest<TppRequestParameters, PatientSelectedReply>
    {
        private readonly ILogger<TppClientPatientSelectedPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientPatientSelectedPost(ILogger<TppClientPatientSelectedPost> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<PatientSelectedReply>> Post(TppRequestParameters tppRequestParameters)
        {
            _logger.LogEnter();

            try
            {
                var patientSelected = new PatientSelected
                {
                    OnlineUserId = tppRequestParameters.OnlineUserId,
                    PatientId = tppRequestParameters.PatientId,
                    UnitId = tppRequestParameters.OdsCode
                };

                return await _requestExecutor.Post<PatientSelectedReply>(
                    requestBuilder => requestBuilder.Model(patientSelected).Suid(tppRequestParameters.Suid));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}