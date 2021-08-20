using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public static class KeyboardFocusStates
    {
        private enum KeyboardFocusVisualStates
        {
            KeyboardUnfocused,
            KeyboardFocused,
            KeyboardSelectedUnfocused,
            KeyboardSelectedFocused
        }

        public static void SetKeyboardFocusState(VisualElement visualElement, bool isKeyboardFocused)
        {
            VisualStateManager.GoToState(visualElement,
                isKeyboardFocused
                    ? nameof(KeyboardFocusVisualStates.KeyboardFocused)
                    : nameof(KeyboardFocusVisualStates.KeyboardUnfocused));
        }

        public static void SetSelectedIconKeyboardFocusState(VisualElement visualElement, bool isSelected, bool isKeyboardFocused)
        {
            var state = (isSelected, isKeyboardFocused) switch
            {
                (true, true) => KeyboardFocusVisualStates.KeyboardSelectedFocused,
                (true, false) => KeyboardFocusVisualStates.KeyboardSelectedUnfocused,
                (false, true) => KeyboardFocusVisualStates.KeyboardFocused,
                (false, false) => KeyboardFocusVisualStates.KeyboardUnfocused
            };

            VisualStateManager.GoToState(visualElement, state.ToString());
        }
    }
}