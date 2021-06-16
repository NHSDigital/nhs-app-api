using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IUpdateRequiredView : INavigationView<IUpdateRequiredView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? OpenAppStoreUrlRequested { get; set; }
        }
    }
}