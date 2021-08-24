using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class LeavePage : NhsAppAlertDialog
    {
        public override string Title => "Leave this page?";
        public override string Message => "If you have entered any information, it will not be saved.";
        public override string AcceptText => "Leave page";
        public override string CancelText => "Cancel";

        public LeavePage(Func<Task> leavePageAction, Func<Task> cancelAction, Func<Task> dismissDialogAction)
        {
            AcceptAction = leavePageAction;
            CancelAction = cancelAction;
            DismissAction = dismissDialogAction;
        }
    }
}