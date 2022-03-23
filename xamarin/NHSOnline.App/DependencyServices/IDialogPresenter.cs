using System.Threading.Tasks;
using NHSOnline.App.Dialogs;

namespace NHSOnline.App.DependencyServices
{
    public interface IDialogPresenter
    {
        bool ShouldShowProminentDialog(string preferenceKey, bool shouldShowRationale);
        Task DisplayAlertDialog(NhsAppAlertDialog nhsAppAlert);
        Task DismissAll();
    }
}