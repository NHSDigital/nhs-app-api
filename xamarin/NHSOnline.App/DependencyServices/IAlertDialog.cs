using System;

namespace NHSOnline.App.DependencyServices
{
    public interface IAlertDialog
    {
        void DisplayAlertDialog(
            string message,
            string acceptText,
            string cancelText,
            Action acceptAction,
            Action cancelAction);

        void DisplayAlertDialog(
            string title,
            string message,
            string acceptText,
            string cancelText,
            Action acceptAction,
            Action cancelAction);

        void DismissAll();
    }
}