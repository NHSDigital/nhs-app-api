using System;
using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public interface IAccessibleControl
    {
        event EventHandler<VisualElement.FocusRequestArgs> AccessibilityFocusChangeRequested;
    }
}