using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public static class KeyboardFocusStates
    {
        private enum KeyboardFocusVisualStates
        {
            KeyboardFocused,
            KeyboardUnfocused
        }

        public static void SetKeyboardFocusState(VisualElement visualElement, bool isKeyboardFocused)
        {
            VisualStateManager.GoToState(visualElement,
                    isKeyboardFocused
                        ? nameof(KeyboardFocusVisualStates.KeyboardFocused)
                        : nameof(KeyboardFocusVisualStates.KeyboardUnfocused));
        }
    }
}