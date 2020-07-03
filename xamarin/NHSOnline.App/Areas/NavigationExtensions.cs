using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas
{
    internal static class NavigationExtensions
    {
        internal static async Task ReplaceCurrentPage(this INavigation navigation, Page page)
        {
            var currentPage = navigation.NavigationStack[^1];

            await navigation.PushAsync(page).PreserveThreadContext();

            navigation.RemovePage(currentPage);
        }

        internal static async Task PopToNewRoot(this INavigation navigation, Page newRootPage)
        {
            navigation.InsertPageBefore(newRootPage, navigation.NavigationStack[0]);
            await navigation.PopToRootAsync().PreserveThreadContext();
        }
    }
}