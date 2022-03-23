using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public class FileStoragePermissionRationale : NhsAppAlertDialog
    {
        public override string Title => "File storage access";
        public override string Message
            => $"The NHS App needs access to your device's file storage to support proof of identity, communication with healthcare providers, and the digital NHS COVID Pass.{Environment.NewLine}Files that you choose to download will be stored on your device.";
        public override string AcceptText => "Allow";
        public override string CancelText => "Cancel";

        public FileStoragePermissionRationale(Func<Task> okAction, Func<Task> cancelAction)
        {
            AcceptAction = okAction;
            CancelAction = cancelAction;
        }
    }
}