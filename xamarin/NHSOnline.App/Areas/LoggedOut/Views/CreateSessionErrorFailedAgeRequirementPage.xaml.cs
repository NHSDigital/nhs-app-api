using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorFailedAgeRequirementPage : ICreateSessionErrorFailedAgeRequirementView
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorFailedAgeRequirementPage), "3c");

        public event EventHandler<EventArgs>? OneOneOneRequested;

        public CreateSessionErrorFailedAgeRequirementPage()
        {
            InitializeComponent();
        }

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public ICommand OneOneOneCommand => new Command(() => OneOneOneRequested?.Invoke(this, EventArgs.Empty));
    }
}
