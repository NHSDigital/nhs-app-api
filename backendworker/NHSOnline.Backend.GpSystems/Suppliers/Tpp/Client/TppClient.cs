using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClient : ITppClient
    {
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccessesPost;
        private readonly ITppClientRequest<TppUserSession, PatientSelectedReply> _patientSelectedPost;
        private readonly ITppClientRequest<TppUserSession, ViewPatientOverviewReply> _patientOverviewPost;
        private readonly ITppClientRequest<TppUserSession, RequestPatientRecordReply> _requestPatientRecordPost;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> _testResultsView;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> _testResultsViewDetailed;
        private readonly ITppClientRequest<TppUserSession, LogoffReply> _logoffPost;
        private readonly ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> _nhsUserPost;
        private readonly ITppClientRequest<TppUserSession, ListRepeatMedicationReply> _listRepeatMedicationPost;
        private readonly ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply> _orderPrescriptionsPost;
        private readonly ITppClientRequest<(RequestSystmOnlineMessages requestModel, string suid), RequestSystmOnlineMessagesReply> _requestSystmOnlineMessages;

        public TppClient(
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccessesPost,
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelectedPost,
            ITppClientRequest<TppUserSession, ViewPatientOverviewReply> patientOverviewPost,
            ITppClientRequest<TppUserSession, RequestPatientRecordReply> requestPatientRecordPost,
            ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> testResultsView,
            ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> testResultsViewDetailed,
            ITppClientRequest<TppUserSession, LogoffReply> logoffPost,
            ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> nhsUserPost,
            ITppClientRequest<TppUserSession, ListRepeatMedicationReply> listRepeatMedicationPost,
            ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply> orderPrescriptionsPost,
            ITppClientRequest<(RequestSystmOnlineMessages requestModel, string suid), RequestSystmOnlineMessagesReply> requestSystmOnlineMessages)
        {
            _listServiceAccessesPost = listServiceAccessesPost;
            _patientSelectedPost = patientSelectedPost;
            _patientOverviewPost = patientOverviewPost;
            _requestPatientRecordPost = requestPatientRecordPost;
            _testResultsView = testResultsView;
            _testResultsViewDetailed = testResultsViewDetailed;
            _logoffPost = logoffPost;
            _nhsUserPost = nhsUserPost;
            _listRepeatMedicationPost = listRepeatMedicationPost;
            _orderPrescriptionsPost = orderPrescriptionsPost;
            _requestSystmOnlineMessages = requestSystmOnlineMessages;
        }

        public async Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(TppUserSession tppUserSession)
            => await _listServiceAccessesPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
            => await _patientSelectedPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(TppUserSession tppUserSession)
            => await _patientOverviewPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(TppUserSession tppUserSession)
            => await _requestPatientRecordPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsView(TppUserSession tppUserSession, string startDate, string endDate)
            =>  await _testResultsView.Post((tppUserSession, startDate, endDate));

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsViewDetailed(TppUserSession tppUserSession, string testResultId)
            => await _testResultsViewDetailed.Post((tppUserSession, testResultId));

        public async Task<TppApiObjectResponse<LogoffReply>> LogoffPost(TppUserSession tppUserSession)
            => await _logoffPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<AddNhsUserResponse>> NhsUserPost(AddNhsUserRequest addNhsUserRequest)
            => await _nhsUserPost.Post(addNhsUserRequest);

        public async Task<TppApiObjectResponse<ListRepeatMedicationReply>> ListRepeatMedicationPost(TppUserSession tppUserSession)
            => await _listRepeatMedicationPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<RequestMedicationReply>> OrderPrescriptionsPost(TppUserSession tppUserSession, RequestMedication requestMedication)
            => await _orderPrescriptionsPost.Post((tppUserSession, requestMedication));

        public async Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> RequestSystmOnlineMessages(RequestSystmOnlineMessages requestModel, string suid)
            => await _requestSystmOnlineMessages.Post((requestModel, suid));
    }
}