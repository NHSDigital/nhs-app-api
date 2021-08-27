using System;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class CloseSlimTryAgainNetworkErrorModel
    {
        public Action RetryAction { get; }

        internal CloseSlimTryAgainNetworkErrorModel(Action retryAction)
        {
            RetryAction = retryAction;
        }
    }
}