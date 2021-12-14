using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IUnsupportedPlatformVersionView : INavigationView<IUnsupportedPlatformVersionView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? CovidPassRequested { get; set; }
            Func<Task>? NhsAppOnlineLoginRequested { get; set; }

        }

        string MinimumPlatformVersion { get; set; }
    }
}