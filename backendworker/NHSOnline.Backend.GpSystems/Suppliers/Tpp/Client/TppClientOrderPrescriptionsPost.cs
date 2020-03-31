using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientOrderPrescriptionsPost
        : ITppClientRequest<(TppRequestParameters tppRequestParameters, RepeatPrescriptionRequest repeatPrescriptionRequest), RequestMedicationReply>
    {
        private readonly ILogger<TppClientOrderPrescriptionsPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientOrderPrescriptionsPost(ILogger<TppClientOrderPrescriptionsPost> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<RequestMedicationReply>> Post((TppRequestParameters tppRequestParameters, RepeatPrescriptionRequest repeatPrescriptionRequest) parameters)
        {
            var (tppRequestParameters, repeatPrescriptionRequest) = parameters;
            var postRequest = new RequestMedication
            {
                PatientId = tppRequestParameters.PatientId,
                OnlineUserId = tppRequestParameters.OnlineUserId,
                UnitId = tppRequestParameters.OdsCode,
                Notes = repeatPrescriptionRequest.SpecialRequest,
                Medications = repeatPrescriptionRequest.CourseIds.Select(x => new MedicationRequest
                {
                    DrugId = x,
                    Type = TppApiConstants.MedicationType.Repeat,
                }).ToList()
            };
            
           return await _requestExecutor.Post<RequestMedicationReply>(requestBuilder => requestBuilder.Model(postRequest).Suid(tppRequestParameters.Suid));
        }
    }
}