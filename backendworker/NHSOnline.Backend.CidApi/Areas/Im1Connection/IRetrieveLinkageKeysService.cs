using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Linkage;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public interface IRetrieveLinkageKeysService
    {
        Task<LinkageResult> RetrieveLinkageKey(RetrieveLinkageKeysRequest model, IGpSystem gpSystem);
    }
}