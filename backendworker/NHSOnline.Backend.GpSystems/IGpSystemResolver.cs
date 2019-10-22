using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystemResolver
    {
        Task<Option<IGpSystem>> ResolveFromOdsCode(string odsCode);
    }
}