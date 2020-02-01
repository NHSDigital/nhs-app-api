using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientOrderPrescriptionsPost
        : ITppClientRequest<(TppUserSession tppUserSession, RequestMedication requestMedication), RequestMedicationReply>
    {
        private readonly ILogger<TppClientOrderPrescriptionsPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientOrderPrescriptionsPost(ILogger<TppClientOrderPrescriptionsPost> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<RequestMedicationReply>> Post(
            (TppUserSession tppUserSession, RequestMedication requestMedication) parameters)
        {
            var (tppUserSession, requestMedication) = parameters;

            return await _requestExecutor.Post<RequestMedicationReply>(
                requestBuilder => requestBuilder.Model(requestMedication).Suid(tppUserSession.Suid));
        }
    }
}