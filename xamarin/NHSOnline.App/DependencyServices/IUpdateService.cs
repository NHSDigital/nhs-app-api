using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices
{
    public interface IUpdateService
    {
        Task OpenAppStoreUrl();
    }
}