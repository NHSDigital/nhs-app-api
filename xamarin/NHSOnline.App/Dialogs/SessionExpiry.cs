using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class SessionExpiry : NhsAppAlertDialog
    {
        public override string? Title => null;
        public override string Message => "For security reasons, we'll log you out of the NHS App in 1 minute.";
        public override string AcceptText => "Stay logged in";
        public override string CancelText => "Log out";

        public SessionExpiry(Func<Task> stayLoggedInAction, Func<Task> logOutAction)
        {
            AcceptAction = stayLoggedInAction;
            CancelAction = logOutAction;
        }
    }
}