using System;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ILoggedOutHomeScreenView
    {
        event EventHandler<EventArgs> LoginRequested;
    }
}