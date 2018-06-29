using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public interface ITppClient
    {
        Task<TppClient.TppApiObjectResponse<AuthenticateReply>> AuthenticatePost(Authenticate model);
        
        Task<TppClient.TppApiObjectResponse<ListRepeatMedicationReply>> ListRepeatMedicationPost(TppUserSession tppUserSession);

        Task<TppClient.TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession session);
    }
}