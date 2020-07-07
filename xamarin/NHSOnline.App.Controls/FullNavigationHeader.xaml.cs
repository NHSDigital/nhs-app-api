using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationHeader
    {
        public static readonly BindableProperty SettingsCommandProperty =
            BindableProperty.Create(nameof(SettingsCommand), typeof(ICommand), typeof(FullNavigationHeader));
        public static readonly BindableProperty HelpCommandProperty =
            BindableProperty.Create(nameof(HelpCommand), typeof(ICommand), typeof(FullNavigationHeader));
        public static readonly BindableProperty HomeCommandProperty =
            BindableProperty.Create(nameof(HomeCommand), typeof(ICommand), typeof(FullNavigationHeader));

        public ICommand SettingsCommand
        {
            get => (ICommand) GetValue(SettingsCommandProperty);
            set => SetValue(SettingsCommandProperty, value);
        }
        public ICommand HelpCommand
        {
            get => (ICommand) GetValue(HelpCommandProperty);
            set => SetValue(HelpCommandProperty, value);
        }
        public ICommand HomeCommand
        {
            get => (ICommand) GetValue(HomeCommandProperty);
            set => SetValue(HomeCommandProperty, value);
        }
        public FullNavigationHeader()
        {
            InitializeComponent();
        }
    }
}
