using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
{
    public interface IMicrotestClient
    {
        Task<MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            string odsCode,
            string nhsNumber,
            AppointmentSlotsDateRange dateRange);
    }
}
