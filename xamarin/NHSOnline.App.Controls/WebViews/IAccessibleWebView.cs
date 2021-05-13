using System;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public interface IAccessibleWebView
    {
        event EventHandler<VisualElement.FocusRequestArgs> AccessibilityFocusChangeRequested;
    }
}