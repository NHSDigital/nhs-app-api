using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBiometricLoginFingerprintFaceIrisFailedView : INavigationView<IBiometricLoginFingerprintFaceIrisFailedView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? BackHomeRequested { get; set; }
        }
    }
}