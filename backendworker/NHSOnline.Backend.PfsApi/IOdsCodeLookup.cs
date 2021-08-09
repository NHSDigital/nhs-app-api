using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi
{
    public interface IOdsCodeLookup
    {
        Task<IEnumerable<string>> LookupOdsCodes();
        Task<Option<Supplier>> LookupSupplier(string odsCode);
    }
}
