using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas
{
    internal static class NavigationExtensions
    {
        internal static async Task ReplaceCurrentPageAsync(this INavigation navigation, Page page)
        {
            var currentPage = navigation.NavigationStack[^1];

            await navigation.PushAsync(page).PreserveThreadContext();

            navigation.RemovePage(currentPage);
        }
    }
}