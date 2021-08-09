using System.Threading.Tasks;

namespace NHSOnline.Backend.Support.AzureManagement
{
    public interface IAzureKeyVaultService
    {
        public Task<ConnectionStringResponse> GetConnectionStrings();
    }
}