using System.Linq;
using System.Threading.Tasks;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyServices.Navigation
{
    // Creating a singleton for the apps navigation to be injected into the page.
    // This way we dont create new instances of app navigation and cause problems where we may get a bit out of sync.
    // Change was due to the issue found in NHSO-18055 where replace current page failed because the page it tried to get from the index didn't exist.
    public class NhsAppNavigationService : INavigationService
    {
        public Task PopToRoot()
            => Application.Current.MainPage.Navigation.PopToRootAsync(false);

        public async Task ReplaceCurrentPage(Page page)
        {
            var currentPage = Application.Current.MainPage.Navigation.NavigationStack[^1];

            await Application.Current.MainPage.Navigation.PushAsync(page, false).PreserveThreadContext();

            if (currentPage?.Id != null
                && Application.Current.MainPage.Navigation.NavigationStack.Any(p => p.Id.Equals(currentPage.Id)))
            {
                Application.Current.MainPage.Navigation.RemovePage(currentPage);
            }
        }

        public Task PopToNewRoot(Page page)
        {
            Application.Current.MainPage.Navigation.InsertPageBefore(
                page,
                Application.Current.MainPage.Navigation.NavigationStack[0]);
            return Application.Current.MainPage.Navigation.PopToRootAsync(false);
        }

        public Task Pop() => Application.Current.MainPage.Navigation.PopAsync(false);

        public Task Push(Page page) => Application.Current.MainPage.Navigation.PushAsync(page, false);
    }
}