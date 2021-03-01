using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Spinner
    {
        public Spinner()
        {
            InitializeComponent();
            var rotationAnimation = new Animation(
                rotation => NhsSpinner.Rotation = 360 * rotation,
                0,
                1,
                Easing.Linear);

            NhsSpinner.Animate("RotateSpinnerAnimation",
                rotationAnimation,
                10,
                1500,
                null,
                null,
                () => true);
        }
    }
}