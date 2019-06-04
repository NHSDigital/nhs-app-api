using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystemFactory
    {
        IGpSystem CreateGpSystem(Supplier supplier);
        Task<Option<IGpSystem>> LookupGpSystem(string odsCode);
    }
}