using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Errors
{
    internal interface IFullNavigationBackToHomeNetworkErrorView: INavigationView<IFullNavigationBackToHomeNetworkErrorView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? HomeRequested { get; set; }
            Func<Task>? HelpRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }
            Func<Task>? AdviceRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? YourHealthRequested { get; set; }
            Func<Task>? MessagesRequested { get; set; }
            Func<Task>? BackToHomeRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
        }

        void SetNavigationFooterItem(NavigationFooterItem footerItem);
    }
}