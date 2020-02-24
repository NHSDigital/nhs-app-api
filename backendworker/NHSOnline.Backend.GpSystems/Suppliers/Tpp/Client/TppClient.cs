using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClient : ITppClient
    {
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccessesPost;
        private readonly ITppClientRequest<TppUserSession, PatientSelectedReply> _patientSelectedPost;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> _testResultsView;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> _testResultsViewDetailed;
        private readonly ITppClientRequest<TppUserSession, LogoffReply> _logoffPost;
        private readonly ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> _nhsUserPost;
        
        public TppClient(
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccessesPost,
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelectedPost,
            ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply> testResultsView,
            ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> testResultsViewDetailed,
            ITppClientRequest<TppUserSession, LogoffReply> logoffPost,
            ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> nhsUserPost)
        {
            _listServiceAccessesPost = listServiceAccessesPost;
            _patientSelectedPost = patientSelectedPost;
            _testResultsView = testResultsView;
            _testResultsViewDetailed = testResultsViewDetailed;
            _logoffPost = logoffPost;
            _nhsUserPost = nhsUserPost;
        }

        public async Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(TppUserSession tppUserSession)
            => await _listServiceAccessesPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
            => await _patientSelectedPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsView(TppUserSession tppUserSession, string startDate, string endDate)
            =>  await _testResultsView.Post((tppUserSession, startDate, endDate));

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsViewDetailed(TppUserSession tppUserSession, string testResultId)
            => await _testResultsViewDetailed.Post((tppUserSession, testResultId));

        public async Task<TppApiObjectResponse<LogoffReply>> LogoffPost(TppUserSession tppUserSession)
            => await _logoffPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<AddNhsUserResponse>> NhsUserPost(AddNhsUserRequest addNhsUserRequest)
            => await _nhsUserPost.Post(addNhsUserRequest);
  }
}