using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class AddToCalendarPermissionDenied : NhsAppAlertDialog
    {
        public override string Title => "Give NHS App calendar access";
        public override string Message => "NHS App does not have permission to add events to your calendar.\nGo to your device settings and allow access, then try again.";
        public override string AcceptText =>  "OK";
        public override string CancelText => "Go to settings";

        public AddToCalendarPermissionDenied(Func<Task> goToSettingsAction)
        {
            CancelAction = goToSettingsAction;
        }
    }
}