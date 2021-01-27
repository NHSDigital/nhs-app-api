using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorUpstreamSystemTimeoutPage : ICreateSessionErrorUpstreamSystemTimeoutView
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorUpstreamSystemTimeoutPage), string.Empty);

        public event EventHandler<EventArgs>? OneOneOneRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;
        public event EventHandler<EventArgs>? BackHomeRequested;

        public CreateSessionErrorUpstreamSystemTimeoutPage()
        {
            InitializeComponent();
        }

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public ICommand OneOneOneCommand => new Command(() => OneOneOneRequested?.Invoke(this, EventArgs.Empty));
        public ICommand ContactUsCommand => new Command(() => ContactUsRequested?.Invoke(this, EventArgs.Empty));
        public ICommand BackHomeCommand => new Command(() => BackHomeRequested?.Invoke(this, EventArgs.Empty));
    }
}
