using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Panels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackToHomeNetworkError
    {
        public static readonly BindableProperty BackToHomeCommandProperty =
            BindableProperty.Create(nameof(BackToHomeCommand), typeof(ICommand), typeof(BackToHomeNetworkError));

        public BackToHomeNetworkError()
        {
            InitializeComponent();
        }

        public ICommand BackToHomeCommand
        {
            get => (ICommand) GetValue(BackToHomeCommandProperty);
            set => SetValue(BackToHomeCommandProperty, value);
        }

        public void AccessibilityFocusHeading()
        {
            Heading.AccessibilityFocus();
        }
    }
}