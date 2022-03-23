using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class LocationPermissionRationale : NhsAppAlertDialog
    {
        public override string Title => "Location access";
        public override string Message => "The NHS App uses location data from your device to find services in your area, only when you're using the app.";
        public override string AcceptText => "Allow";
        public override string CancelText => "Cancel";

        public LocationPermissionRationale(Func<Task> okAction, Func<Task> cancelAction)
        {
            AcceptAction = okAction;
            CancelAction = cancelAction;
        }
    }
}