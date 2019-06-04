using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse,  
            DemographicsGetResponse demographicsResponse );
    }
}