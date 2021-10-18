using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBiometricLoginLegacySensorNotValidView : INavigationView<IBiometricLoginLegacySensorNotValidView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Func<Task>? BackToLoginRequested { get; set; }
        }
    }
}