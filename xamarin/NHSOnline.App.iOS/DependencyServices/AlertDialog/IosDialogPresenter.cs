using System.Threading.Tasks;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.Controllers;
using NHSOnline.App.iOS.DependencyServices.AlertDialog;
using NHSOnline.App.Threading;
using Xamarin.Forms;


[assembly: Dependency(typeof(IosDialogPresenter))]
namespace NHSOnline.App.iOS.DependencyServices.AlertDialog
{
    public class IosDialogPresenter : IDialogPresenter
    {
        public async Task DisplayAlertDialog(Dialogs.NhsAppAlertDialog nhsAppAlert)
        {
            await AlertDialogBoxController.ShowAlertPopup(nhsAppAlert).ResumeOnThreadPool();
        }

        public async Task DismissAll()
        {
            await AlertDialogBoxController.DismissAll().ResumeOnThreadPool();
        }
    }
}