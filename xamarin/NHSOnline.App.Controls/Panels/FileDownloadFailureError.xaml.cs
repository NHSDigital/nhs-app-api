using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Panels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileDownloadFailureError
    {
        public static readonly BindableProperty TryAgainCommandProperty =
            BindableProperty.Create(nameof(TryAgainCommand), typeof(ICommand), typeof(FileDownloadFailureError));

        public FileDownloadFailureError()
        {
            InitializeComponent();
        }

        public ICommand TryAgainCommand
        {
            get => (ICommand) GetValue(TryAgainCommandProperty);
            set => SetValue(TryAgainCommandProperty, value);
        }

        public void AccessibilityFocusHeading() => Heading.AccessibilityFocus();
    }
}