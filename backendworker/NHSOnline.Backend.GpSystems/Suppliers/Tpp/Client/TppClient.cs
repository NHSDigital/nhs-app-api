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
        private readonly
            ITppClientRequest<(TppUserSession tppUserSession, string documentIdentifier),
                RequestBinaryDataReply> _requestBinaryData;

        public TppClient(
            ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccessesPost,
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelectedPost,
            ITppClientRequest<(TppUserSession tppUserSession, string documentIdentifier),
            RequestBinaryDataReply> requestBinaryData)
            {
            _listServiceAccessesPost = listServiceAccessesPost;
            _patientSelectedPost = patientSelectedPost;
            _requestBinaryData = requestBinaryData;
        }

        public async Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(
            TppUserSession tppUserSession)
            => await _listServiceAccessesPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
            => await _patientSelectedPost.Post(tppUserSession);

        public async Task<TppApiObjectResponse<RequestBinaryDataReply>> RequestBinaryData(
            string documentIdentifier, TppUserSession tppUserSession)
            => await _requestBinaryData.Post((tppUserSession, documentIdentifier));
    }
}