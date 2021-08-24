using System.Threading.Tasks;
using NHSOnline.App.Dialogs;

namespace NHSOnline.App.DependencyServices
{
    public interface IDialogPresenter
    {
        Task DisplayAlertDialog(NhsAppAlertDialog nhsAppAlert);
        Task DismissAll();
    }
}