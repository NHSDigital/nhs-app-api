using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(ListItemContent))]
    public partial class ListItem
    {
        public static readonly BindableProperty ListItemContentProperty =
            BindableProperty.Create(nameof(ListItemContent), typeof(View), typeof(ListItem));

        public static readonly BindableProperty CommandProperty = TapGestureRecognizer.CommandProperty;

        public ListItem()
        {
            InitializeComponent();
        }

        public View ListItemContent
        {
            get => (View) GetValue(ListItemContentProperty);
            set => SetValue(ListItemContentProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}