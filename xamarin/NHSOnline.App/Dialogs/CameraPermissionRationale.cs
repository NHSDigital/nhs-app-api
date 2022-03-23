using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class CameraPermissionRationale : NhsAppAlertDialog
    {
        public override string Title => "Camera access";
        public override string Message => "The NHS App uses data from your device's camera to support proof of identity. We do not store this data.";
        public override string AcceptText => "Allow";
        public override string CancelText => "Cancel";

        public CameraPermissionRationale(Func<Task> okAction, Func<Task> cancelAction)
        {
            AcceptAction = okAction;
            CancelAction = cancelAction;
        }
    }
}