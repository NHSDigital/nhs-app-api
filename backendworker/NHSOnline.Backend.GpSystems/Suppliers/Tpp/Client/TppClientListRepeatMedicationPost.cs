using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientListRepeatMedicationPost : ITppClientRequest<TppRequestParameters, ListRepeatMedicationReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientListRepeatMedicationPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ListRepeatMedicationReply>> Post(TppRequestParameters tppRequestParameters)
        {
            var listRepeatMedication = new ListRepeatMedication
            {
                PatientId = tppRequestParameters.PatientId,
                OnlineUserId = tppRequestParameters.OnlineUserId,
                UnitId = tppRequestParameters.OdsCode,
            };

            return await _requestExecutor.Post<ListRepeatMedicationReply>(
                requestBuilder => requestBuilder.Model(listRepeatMedication).Suid(tppRequestParameters.Suid));
        }
    }
}