using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public partial class LinkSpan
    {
        public static readonly BindableProperty TappedProperty =
            BindableProperty.Create(nameof(Tapped), typeof(ICommand), typeof(LinkSpan));

        public LinkSpan()
        {
            InitializeComponent();

            TextDecorations = TextDecorations.Underline;
            TextColor = NhsUkColours.NhsUkPrimaryLinkBlue;
        }

        public ICommand Tapped
        {
            get => (ICommand)GetValue(TappedProperty);
            set => SetValue(TappedProperty, value);
        }
    }
}
