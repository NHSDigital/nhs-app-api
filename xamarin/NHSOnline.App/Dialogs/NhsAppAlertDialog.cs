using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Dialogs
{
    public abstract class NhsAppAlertDialog
    {
        public abstract string? Title { get; }
        public abstract string Message { get; }
        public abstract string AcceptText { get; }
        public abstract string CancelText { get; }
        public Func<Task> AcceptAction { get; protected set; }
        public Func<Task> CancelAction { get; protected set; }
        public Func<Task> DismissAction { get; protected set; }

        protected NhsAppAlertDialog()
        {
            AcceptAction = NoOperation;
            CancelAction = NoOperation;
            DismissAction = NoOperation;
        }

        public bool HasTitleText => Title != null;

        private static Task NoOperation()
        {
            return Task.CompletedTask;
        }
    }
}