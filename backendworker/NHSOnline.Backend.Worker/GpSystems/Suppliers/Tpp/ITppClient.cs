using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.TppClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public interface ITppClient
    {

        Task<TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(Authenticate model);

        Task<TppApiObjectResponse<LinkAccountReply>> LinkAccountPost(LinkAccount linkAccountModel);
        
        Task<TppApiObjectResponse<ListRepeatMedicationReply>> ListRepeatMedicationPost(TppUserSession tppUserSession);

        Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession session);
        
        Task<TppApiObjectResponse<ListSlotsReply>> ListSlotsPost(ListSlots request, string suid);
        
        Task<TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(TppUserSession tppUserSession);

        Task<TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(TppUserSession tppUserSession);

        Task<TppApiObjectResponse<BookAppointmentReply>> BookAppointmentSlotPost(BookAppointment bookAppointment, TppUserSession userSession);
        
        Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsView(TppUserSession tppUserSession, string startDate, string endDate);

        Task<TppApiObjectResponse<ViewAppointmentsReply>> ViewAppointmentsPost(ViewAppointments request, string suid);

        Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsViewDetailed(TppUserSession tppUserSession, string testResultId);
        
        Task<TppApiObjectResponse<CancelAppointmentReply>> CancelAppointmentPost(CancelAppointment model, string suid);

        Task<TppApiObjectResponse<RequestMedicationReply>> OrderPrescriptionsPost(TppUserSession tppUserSession, RequestMedication requestMedication);
    }
}
