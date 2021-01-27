using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage : ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage), "3f");

        public event EventHandler<EventArgs>? MyHealthOnlineRequested;
        public event EventHandler<EventArgs>? OneOneOneWalesRequested;
        public event EventHandler<EventArgs>? OneOneOneRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;

        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage()
        {
            InitializeComponent();
        }

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public ICommand MyHealthOnlineCommand => new Command(() => MyHealthOnlineRequested?.Invoke(this, EventArgs.Empty));
        public ICommand OneOneOneWalesCommand => new Command(() => OneOneOneWalesRequested?.Invoke(this, EventArgs.Empty));
        public ICommand OneOneOneCommand => new Command(() => OneOneOneRequested?.Invoke(this, EventArgs.Empty));
        public ICommand ContactUsCommand => new Command(() => ContactUsRequested?.Invoke(this, EventArgs.Empty));
    }
}
