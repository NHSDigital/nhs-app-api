using System;

namespace NHSOnline.App.Controls
{
    public interface IVisibleControl
    {
        event EventHandler<VisibilityChangeEventArgs> VisibilityChangeRequested;

        public class VisibilityChangeEventArgs : EventArgs
        {
            public bool IsVisible { get; set; }
        }
    }
}