using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Expander
    {
        public static readonly BindableProperty HeaderTextProperty =
            BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(Button));
        public static readonly BindableProperty BodyTextProperty =
            BindableProperty.Create(nameof(BodyText), typeof(string), typeof(Button));

        public string HeaderText
        {
            get => (string) GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }
        public string BodyText
        {
            get => (string) GetValue(BodyTextProperty);
            set => SetValue(BodyTextProperty, value);
        }
        public Expander()
        {
            InitializeComponent();
        }
    }
}