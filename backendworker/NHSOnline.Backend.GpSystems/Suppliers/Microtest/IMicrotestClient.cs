using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;

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
            string nhsNumber,
            DateTimeOffset? pastAppointmentsFromDate);

        Task<MicrotestClient.MicrotestApiResponse> AppointmentsPost(
            string odsCode,
            string nhsNumber,
            BookAppointmentSlotPostRequest bookAppointmentSlotPostRequest);

        Task<MicrotestClient.MicrotestApiResponse> AppointmentsDelete(
            string odsCode,
            string nhsNumber,
            CancelAppointmentDeleteRequest cancelAppointmentDeleteRequest);

        Task<MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>> DemographicsGet(
            string odsCode,
            string nhsNumber);
        
        Task<MicrotestClient.MicrotestApiObjectResponse<PatientRecordGetResponse>> MedicalRecordGet(
            string odsCode,
            string nhsNumber);

        Task<MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>> PrescriptionHistoryGet(
            string odsCode,
            string nhsNumber,
            DateTimeOffset? fromDate);

        Task<MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>> CoursesGet(
            string odsCode,
            string nhsNumber);

        Task<MicrotestClient.MicrotestApiObjectResponse<PrescriptionOrderResponse>> PrescriptionsPost(
            string odsCode,
            string nhsNumber,
            PrescriptionRequestsPost model);
    }
}
