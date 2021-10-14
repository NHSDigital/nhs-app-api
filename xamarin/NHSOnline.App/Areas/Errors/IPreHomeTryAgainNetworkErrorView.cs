using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors
{
    internal interface IPreHomeTryAgainNetworkErrorView: INavigationView<IPreHomeTryAgainNetworkErrorView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? TryAgainRequested { get; set; }
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
        }
    }
}