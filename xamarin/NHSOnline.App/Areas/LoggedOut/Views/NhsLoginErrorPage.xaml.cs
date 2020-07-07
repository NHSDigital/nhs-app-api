using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginErrorPage : INhsLoginErrorView
    {
        public event EventHandler<EventArgs>? BackHomeRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;

        public NhsLoginErrorPage()
        {
            InitializeComponent();
        }

        public ICommand BackHomeCommand => new Command(() => BackHomeRequested?.Invoke(this, EventArgs.Empty));
        public ICommand ContactUsCommand => new Command(() => ContactUsRequested?.Invoke(this, EventArgs.Empty));
    }
}
