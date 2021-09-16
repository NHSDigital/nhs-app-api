using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors
{
    internal interface ICloseSlimTryAgainNetworkErrorView: INavigationView<ICloseSlimTryAgainNetworkErrorView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? CloseRequested { get; set; }
            Func<Task>? TryAgainRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
        }
    }
}