using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public interface ITppClient
    {
        Task<TppClient.TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(Authenticate model);
        
        Task<TppClient.TppApiObjectResponse<ListRepeatMedicationReply>> ListRepeatMedicationPost(TppUserSession tppUserSession);

        Task<TppClient.TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession session);
        
        Task<TppClient.TppApiObjectResponse<ListSlotsReply>> ListSlotsPost(ListSlots request, string suid);
        
        Task<TppClient.TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(TppUserSession tppUserSession);

        Task<TppClient.TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(TppUserSession tppUserSession);

        Task<TppClient.TppApiObjectResponse<BookAppointmentReply>> BookAppointmentSlotPost(BookAppointment bookAppointment, TppUserSession userSession);
    }
}