using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public interface IOdsCodeLookup
    {
        Task<Option<Supplier>> LookupSupplier(string odsCode);
    }
}