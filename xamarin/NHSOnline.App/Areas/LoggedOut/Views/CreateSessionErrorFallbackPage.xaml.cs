using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorFallbackPage : ICreateSessionErrorFallbackView
    {
        public event EventHandler<EventArgs>? OneOneOneRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;
        public event EventHandler<EventArgs>? BackHomeRequested;

        public CreateSessionErrorFallbackPage()
        {
            InitializeComponent();
        }

        public ICommand OneOneOneCommand => new Command(() => OneOneOneRequested?.Invoke(this, EventArgs.Empty));
        public ICommand ContactUsCommand => new Command(() => ContactUsRequested?.Invoke(this, EventArgs.Empty));
        public ICommand BackHomeCommand => new Command(() => BackHomeRequested?.Invoke(this, EventArgs.Empty));
    }
}
