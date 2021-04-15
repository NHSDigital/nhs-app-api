using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBiometricLoginFaceIdFailedView : INavigationView<IBiometricLoginFaceIdFailedView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? BackHomeRequested { get; set; }
        }
    }
}