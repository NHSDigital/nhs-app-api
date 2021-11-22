using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Spinner : IAccessibleControl, IVisibleControl
    {
        private const string AnimationName = "RotateSpinnerAnimation";

        public event EventHandler<FocusRequestArgs>? AccessibilityFocusChangeRequested;
        public event EventHandler<IVisibleControl.VisibilityChangeEventArgs>? VisibilityChangeRequested;

        public Spinner()
        {
            InitializeComponent();
            StartAnimation();
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            if (propertyName == "IsVisible")
            {
                VisibilityChanged(IsVisible);

                if (!IsVisible)
                {
                    NhsSpinner.CancelAnimations();
                }

                if (!NhsSpinner.AnimationIsRunning(AnimationName))
                {
                    StartAnimation();
                }
            }
        }

        private void StartAnimation()
        {
            var rotationAnimation = new Animation(
                rotation => NhsSpinner.Rotation = 360 * rotation,
                0,
                1,
                Easing.Linear);

            NhsSpinner.Animate(AnimationName,
                rotationAnimation,
                10,
                1500,
                null,
                null,
                () => true);
        }

        public void AccessibilityFocus()
        {
            if (AccessibilityFocusChangeRequested != null)
            {
                var arg = new FocusRequestArgs {Focus = true};
                AccessibilityFocusChangeRequested(this, arg);
            }
        }

        private void VisibilityChanged(bool isVisible)
        {
            if (VisibilityChangeRequested != null)
            {
                var arg = new IVisibleControl.VisibilityChangeEventArgs {IsVisible = isVisible};
                VisibilityChangeRequested(this, arg);
            }
        }
    }
}