using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class ConfirmLogout : NhsAppAlertDialog
    {
        public override string? Title => null;
        public override string Message => "Are you sure you want to log out?";
        public override string AcceptText => "Log out";
        public override string CancelText => "Cancel";

        public ConfirmLogout(Func<Task> logoutAction)
        {
            AcceptAction = logoutAction;
        }
    }
}