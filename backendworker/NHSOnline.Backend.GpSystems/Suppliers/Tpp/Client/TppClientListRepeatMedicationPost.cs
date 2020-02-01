using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientListRepeatMedicationPost : ITppClientRequest<TppUserSession, ListRepeatMedicationReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientListRepeatMedicationPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ListRepeatMedicationReply>> Post(TppUserSession tppUserSession)
        {
            var listRepeatMedication = new ListRepeatMedication
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
            };

            return await _requestExecutor.Post<ListRepeatMedicationReply>(
                requestBuilder => requestBuilder.Model(listRepeatMedication).Suid(tppUserSession.Suid));
        }
    }
}