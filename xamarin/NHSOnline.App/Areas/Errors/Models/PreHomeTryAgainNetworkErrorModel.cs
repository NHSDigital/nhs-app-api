using System;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class PreHomeTryAgainNetworkErrorModel
    {
        public Action RetryAction { get; }

        internal PreHomeTryAgainNetworkErrorModel(Action retryAction)
        {
            RetryAction = retryAction;
        }
    }
}