using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClient : ITppClient
    {
        private readonly ITppClientRequest<TppUserSession, PatientSelectedReply> _patientSelectedPost;


        public TppClient(
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelectedPost)
            {
            _patientSelectedPost = patientSelectedPost;
        }

        public async Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession)
            => await _patientSelectedPost.Post(tppUserSession);
    }
}