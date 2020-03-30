using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestPatientRecordPost : ITppClientRequest<TppRequestParameters, RequestPatientRecordReply>
    {
        private readonly ILogger<TppClientRequestPatientRecordPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientRequestPatientRecordPost(
            ILogger<TppClientRequestPatientRecordPost> logger,
            TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<RequestPatientRecordReply>> Post(TppRequestParameters tppRequestParameters)
        {
            _logger.LogEnter();

            try
            {
                var request = new RequestPatientRecord
                {
                    PatientId = tppRequestParameters.PatientId,
                    OnlineUserId = tppRequestParameters.OnlineUserId,
                    UnitId = tppRequestParameters.OdsCode,
                };

                return await _requestExecutor.Post<RequestPatientRecordReply>(
                    requestBuilder => requestBuilder.Model(request).Suid(tppRequestParameters.Suid));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}