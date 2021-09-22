using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas.Errors.Models
{
    internal class CloseSlimBackToHomeNetworkErrorModel
    {
        public Func<Task> BackToHomeAction { get; }

        internal CloseSlimBackToHomeNetworkErrorModel(Func<Task> backToHomeAction)
        {
            BackToHomeAction = backToHomeAction;
        }
    }
}