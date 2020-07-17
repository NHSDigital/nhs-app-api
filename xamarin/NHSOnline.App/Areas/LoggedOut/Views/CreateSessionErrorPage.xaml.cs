using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorPage : ICreateSessionErrorView
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorPage), "3h");

        public event EventHandler<EventArgs>? BackHomeRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;

        public CreateSessionErrorPage()
        {
            InitializeComponent();
        }

        public string ServiceDeskReference
        {
            get => (string)GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public ICommand BackHomeCommand => new Command(() => BackHomeRequested?.Invoke(this, EventArgs.Empty));
        public ICommand ContactUsCommand => new Command(() => ContactUsRequested?.Invoke(this, EventArgs.Empty));
    }
}
