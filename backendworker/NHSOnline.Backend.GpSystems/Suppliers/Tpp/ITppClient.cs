using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public interface ITppClient
    {
        Task<TppApiObjectResponse<ListServiceAccessesReply>> ListServiceAccessesPost(TppUserSession tppUserSession);

        Task<TppApiObjectResponse<PatientSelectedReply>> PatientSelectedPost(TppUserSession tppUserSession);
    }
}