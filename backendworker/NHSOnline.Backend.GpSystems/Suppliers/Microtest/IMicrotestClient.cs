using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public interface IMicrotestClient
    {
        Task<MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            string odsCode,
            string nhsNumber,
            AppointmentSlotsDateRange dateRange);

        Task<MicrotestClient.MicrotestApiObjectResponse<string>> BookAppointmentSlotPost(
            BookAppointmentSlotPostRequest bookAppointmentSlotPostRequest,
            MicrotestUserSession userSession);
    }
}
