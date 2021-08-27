using System;

namespace NHSOnline.App.Events.Models
{
    public sealed class NavigationFailedArgs
    {
        public Uri FailedUrl { get; }
        public bool OnInitialNavigation { get; }

        public NavigationFailedArgs(Uri failedUrl, bool onInitialNavigation)
        {
            FailedUrl = failedUrl;
            OnInitialNavigation = onInitialNavigation;
        }
    }
}