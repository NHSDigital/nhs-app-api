using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas
{
    public interface INhsAppPage
    {
        Task HandleDeeplink(Uri deeplinkUrl);
    }
}