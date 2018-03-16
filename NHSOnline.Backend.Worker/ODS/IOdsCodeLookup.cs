using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Suppliers;

namespace NHSOnline.Backend.Worker.Ods
{
    public interface IOdsCodeLookup
    {
        Task<SupplierEnum> LookupSupplier(string odsCode);
    }
}