using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors
{
    internal interface ICloseSlimBackToHomeNetworkErrorView: INavigationView<ICloseSlimBackToHomeNetworkErrorView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? BackToHomeRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
        }
    }
}