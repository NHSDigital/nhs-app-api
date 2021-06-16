using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IUpdateCheckFailedView : INavigationView<IUpdateCheckFailedView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? BackToHomeRequested { get; set; }
        }
    }
}