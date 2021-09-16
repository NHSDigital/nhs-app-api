using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class CloseSlimTryAgainNetworkErrorModel
    {
        public Func<Task> CloseAction { get; }
        public Action RetryAction { get; }

        internal CloseSlimTryAgainNetworkErrorModel(Func<Task> closeAction, Action retryAction)
        {
            CloseAction = closeAction;
            RetryAction = retryAction;
        }
    }
}