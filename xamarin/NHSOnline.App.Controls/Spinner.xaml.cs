using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Spinner
    {

        public const string AnimationName = "RotateSpinnerAnimation";

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
    }
}