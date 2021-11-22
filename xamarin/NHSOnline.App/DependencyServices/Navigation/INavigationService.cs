using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyServices.Navigation
{
    public interface INavigationService
    {
        Task PopToRoot();
        Task ReplaceCurrentPage(Page page);
        Task PopToNewRoot(Page page);
        Task Pop();
        Task Push(Page page);
    }
}