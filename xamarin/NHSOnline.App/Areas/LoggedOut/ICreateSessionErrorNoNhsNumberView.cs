using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorNoNhsNumberView: INavigationView<ICreateSessionErrorNoNhsNumberView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? ContactUsRequested { get; set; }
        }
    }
}