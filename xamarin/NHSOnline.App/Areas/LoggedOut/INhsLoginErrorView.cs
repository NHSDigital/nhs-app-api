using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginErrorView: INavigationView<INhsLoginErrorView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? BackHomeRequested { get; set; }
            Func<Task>? ContactUsRequested { get; set; }
        }
    }
}