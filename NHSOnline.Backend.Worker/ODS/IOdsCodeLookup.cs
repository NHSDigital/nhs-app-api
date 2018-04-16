using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Ods
{
    public interface IOdsCodeLookup
    {
        Task<Option<SupplierEnum>> LookupSupplier(string odsCode);
    }
}
