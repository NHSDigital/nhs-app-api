using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlimBackNavigationHeader
    {

        public static readonly BindableProperty CommandProperty = TapGestureRecognizer.CommandProperty;

        public SlimBackNavigationHeader()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private async void BackArrowClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync().ConfigureAwait(true);
        }
    }
}
