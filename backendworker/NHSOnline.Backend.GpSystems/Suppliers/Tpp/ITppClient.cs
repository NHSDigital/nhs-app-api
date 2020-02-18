using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public interface ITppClient
    {
        
        Task<TppApiObjectResponse<ListRepeatMedicationReply>> ListRepeatMedicationPost(TppUserSession tppUserSession);
        
        Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(TppUserSession tppUserSession);

        Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession);
        
        Task<TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(TppUserSession tppUserSession);

        Task<TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(TppUserSession tppUserSession);
        
        Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsView(TppUserSession tppUserSession, string startDate, string endDate);
        
        Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsViewDetailed(TppUserSession tppUserSession, string testResultId);

        Task<TppApiObjectResponse<RequestMedicationReply>> OrderPrescriptionsPost(TppUserSession tppUserSession, RequestMedication requestMedication);

        Task<TppApiObjectResponse<LogoffReply>> LogoffPost(TppUserSession tppUserSession);
        
        Task<TppApiObjectResponse<AddNhsUserResponse>> NhsUserPost(AddNhsUserRequest addNhsUserRequest);
        
        Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> RequestSystmOnlineMessages(RequestSystmOnlineMessages requestModel, string suid);
    }
}
