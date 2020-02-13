using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClient : ITppClient
    {
        private readonly ITppClientRequest<Authenticate, AuthenticateReply> _authenticatePost;
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccessesPost;
        private readonly ITppClientRequest<TppUserSession, PatientSelectedReply> _patientSelectedPost;
        private readonly ITppClientRequest<TppUserSession, ViewPatientOverviewReply> _patientOverviewPost;
        private readonly ITppClientRequest<TppUserSession, RequestPatientRecordReply> _requestPatientRecordPost;
        private readonly ITppClientRequest<(TppUserSession userSession, BookAppointment bookAppointment), BookAppointmentReply> _bookAppointmentSlotPost;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> _testResultsView;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> _testResultsViewDetailed;
        private readonly ITppClientRequest<TppUserSession, LogoffReply> _logoffPost;
        private readonly ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> _nhsUserPost;
        private readonly ITppClientRequest<TppUserSession, ListRepeatMedicationReply> _listRepeatMedicationPost;
        private readonly ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply> _orderPrescriptionsPost;
        private readonly ITppClientRequest<(ListSlots listSlots, string suid), ListSlotsReply> _listSlotsPost;
        private readonly ITppClientRequest<(ViewAppointments viewAppointments, string suid), ViewAppointmentsReply> _viewAppointmentsPost;
        private readonly ITppClientRequest<(CancelAppointment cancelAppointment, string suid), CancelAppointmentReply> _cancelAppointmentPost;
        private readonly ITppClientRequest<(RequestSystmOnlineMessages requestModel, string suid), RequestSystmOnlineMessagesReply> _requestSystmOnlineMessages;

        public TppClient(
            ITppClientRequest<Authenticate, AuthenticateReply> authenticatePost,
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccessesPost,
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelectedPost,
            ITppClientRequest<TppUserSession, ViewPatientOverviewReply> patientOverviewPost,
            ITppClientRequest<TppUserSession, RequestPatientRecordReply> requestPatientRecordPost,
            ITppClientRequest<(TppUserSession userSession, BookAppointment bookAppointment), BookAppointmentReply> bookAppointmentSlotPost,
            ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> testResultsView,
            ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> testResultsViewDetailed,
            ITppClientRequest<TppUserSession, LogoffReply> logoffPost,
            ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> nhsUserPost,
            ITppClientRequest<TppUserSession, ListRepeatMedicationReply> listRepeatMedicationPost,
            ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply> orderPrescriptionsPost,
            ITppClientRequest<(ListSlots listSlots, string suid), ListSlotsReply> listSlotsPost,
            ITppClientRequest<(ViewAppointments viewAppointments, string suid), ViewAppointmentsReply> viewAppointmentsPost,
            ITppClientRequest<(CancelAppointment cancelAppointment, string suid), CancelAppointmentReply> cancelAppointmentPost,
            ITppClientRequest<(RequestSystmOnlineMessages requestModel, string suid), RequestSystmOnlineMessagesReply> requestSystmOnlineMessages)
        {
            _authenticatePost = authenticatePost;
            _listServiceAccessesPost = listServiceAccessesPost;
            _patientSelectedPost = patientSelectedPost;
            _patientOverviewPost = patientOverviewPost;
            _requestPatientRecordPost = requestPatientRecordPost;
            _bookAppointmentSlotPost = bookAppointmentSlotPost;
            _testResultsView = testResultsView;
            _testResultsViewDetailed = testResultsViewDetailed;
            _logoffPost = logoffPost;
            _nhsUserPost = nhsUserPost;
            _listRepeatMedicationPost = listRepeatMedicationPost;
            _orderPrescriptionsPost = orderPrescriptionsPost;
            _listSlotsPost = listSlotsPost;
            _viewAppointmentsPost = viewAppointmentsPost;
            _cancelAppointmentPost = cancelAppointmentPost;
            _requestSystmOnlineMessages = requestSystmOnlineMessages;
        }

        public async Task<TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(Authenticate authenticate)
            => await _authenticatePost.Post(authenticate);

        public async Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(TppUserSession tppUserSession)
            => await _listServiceAccessesPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
            => await _patientSelectedPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<ViewPatientOverviewReply>> PatientOverviewPost(TppUserSession tppUserSession)
            => await _patientOverviewPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<RequestPatientRecordReply>> RequestPatientRecordPost(TppUserSession tppUserSession)
            => await _requestPatientRecordPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<BookAppointmentReply>> BookAppointmentSlotPost(BookAppointment bookAppointment, TppUserSession userSession)
            => await _bookAppointmentSlotPost.Post((userSession, bookAppointment));

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

        public async Task<TppApiObjectResponse<ListSlotsReply>> ListSlotsPost(ListSlots listSlots, string suid)
            => await _listSlotsPost.Post((listSlots, suid));

        public async Task<TppApiObjectResponse<ViewAppointmentsReply>> ViewAppointmentsPost(ViewAppointments viewAppointments, string suid)
            =>  await _viewAppointmentsPost.Post((viewAppointments, suid));

        public async Task<TppApiObjectResponse<CancelAppointmentReply>> CancelAppointmentPost(CancelAppointment cancelAppointment, string suid)
            => await _cancelAppointmentPost.Post((cancelAppointment, suid));

        public async Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> RequestSystmOnlineMessages(RequestSystmOnlineMessages requestModel, string suid)
            => await _requestSystmOnlineMessages.Post((requestModel, suid));
    }
}