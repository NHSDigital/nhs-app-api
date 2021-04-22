using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBiometricLoginTouchIdLockedOutView : INavigationView<IBiometricLoginTouchIdLockedOutView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Func<Task>? BackHomeRequested { get; set; }
        }
    }
}