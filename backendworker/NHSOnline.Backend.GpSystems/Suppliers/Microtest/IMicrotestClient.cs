using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public interface IMicrotestClient
    {
        Task<MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            string odsCode,
            string nhsNumber,
            AppointmentSlotsDateRange dateRange);

        Task<MicrotestClient.MicrotestApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            string odsCode,
            string nhsNumber);

        Task<MicrotestClient.MicrotestApiObjectResponse<string>> AppointmentsPost(
            string odsCode,
            string nhsNumber,
            BookAppointmentSlotPostRequest bookAppointmentSlotPostRequest);

        Task<MicrotestClient.MicrotestApiObjectResponse<string>> AppointmentsDelete(
            string odsCode,
            string nhsNumber,
            CancelAppointmentDeleteRequest cancelAppointmentDeleteRequest);

        Task<MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>> DemographicsGet(
            string odsCode,
            string nhsNumber);
    }
}
