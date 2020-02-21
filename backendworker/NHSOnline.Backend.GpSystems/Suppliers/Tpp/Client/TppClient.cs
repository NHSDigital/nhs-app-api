using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClient : ITppClient
    {
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccessesPost;
        private readonly ITppClientRequest<TppUserSession, PatientSelectedReply> _patientSelectedPost;
        private readonly ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> _testResultsViewDetailed;
        private readonly ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> _nhsUserPost;
        private readonly
            ITppClientRequest<(RequestBinaryData requestBinaryData, TppUserSession tppUserSession),
                RequestBinaryDataReply> _requestBinaryData;

        public TppClient(
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccessesPost,
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelectedPost,
            ITppClientRequest<(TppUserSession tppUserSession, string testResultId), TestResultsViewReply> testResultsViewDetailed,
            ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> nhsUserPost,
            ITppClientRequest<(RequestBinaryData requestBinaryData, TppUserSession tppUserSession),
            RequestBinaryDataReply> requestBinaryData)
            {
            _listServiceAccessesPost = listServiceAccessesPost;
            _patientSelectedPost = patientSelectedPost;
            _testResultsViewDetailed = testResultsViewDetailed;
            _nhsUserPost = nhsUserPost;
            _requestBinaryData = requestBinaryData;
        }

        public async Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(
            TppUserSession tppUserSession)
            => await _listServiceAccessesPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
            => await _patientSelectedPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<TestResultsViewReply>> TestResultsViewDetailed(TppUserSession tppUserSession, string testResultId)
            => await _testResultsViewDetailed.Post((tppUserSession, testResultId));

        public async Task<TppApiObjectResponse<AddNhsUserResponse>> NhsUserPost(AddNhsUserRequest addNhsUserRequest)
            => await _nhsUserPost.Post(addNhsUserRequest);

        public async Task<TppApiObjectResponse<RequestBinaryDataReply>> RequestBinaryData(
            RequestBinaryData requestBinaryData, TppUserSession tppUserSession)
            => await _requestBinaryData.Post((requestBinaryData, tppUserSession));

    }
}