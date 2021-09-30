using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IUpdateCheckFailedView : INavigationView<IUpdateCheckFailedView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? BackToLoginRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
        }
    }
}