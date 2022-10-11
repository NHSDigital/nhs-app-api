using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class KeywordReplyLeavePage : NhsAppAlertDialog
    {
        public override string Title => "You're about to leave this page.";
        public override string Message => "Your reply to this message will not be sent.";
        public override string AcceptText => "Leave page";
        public override string CancelText => "Cancel";

        public KeywordReplyLeavePage(Func<Task> leavePageAction, Func<Task> cancelAction,
            Func<Task> dismissDialogAction)
        {
            AcceptAction = leavePageAction;
            CancelAction = cancelAction;
            DismissAction = dismissDialogAction;
        }
    }
}