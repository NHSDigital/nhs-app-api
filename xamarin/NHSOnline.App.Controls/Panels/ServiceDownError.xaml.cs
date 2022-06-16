using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Panels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceDownError
    {
        public static readonly BindableProperty OneOneOneCommandProperty =
            BindableProperty.Create(nameof(OneOneOneCommand), typeof(ICommand), typeof(ServiceDownError));

        public ServiceDownError()
        {
            InitializeComponent();
        }

        public ICommand OneOneOneCommand
        {
            get => (ICommand) GetValue(OneOneOneCommandProperty);
            set => SetValue(OneOneOneCommandProperty, value);
        }
    }
}