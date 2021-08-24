using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class AddToCalendarValidationFailed : NhsAppAlertDialog
    {
        public override string? Title => "Cannot save event";
        public override string Message => "You can try adding the event to your calendar yourself.";
        public override string AcceptText => "OK";
        public override string CancelText => "Add event";

        public AddToCalendarValidationFailed(Func<Task> addEventAction)
        {
            CancelAction = addEventAction;
        }
    }
}