using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginTermsAndConditionsDeclinedPage : INhsLoginTermsAndConditionsDeclinedView
    {
        public event EventHandler<EventArgs>? BackToHomeRequested;
        public event EventHandler<EventArgs>? OneOneOneRequested;

        public NhsLoginTermsAndConditionsDeclinedPage()
        {
            InitializeComponent();
        }

        public ICommand BackToHomeCommand => new Command(() => BackToHomeRequested?.Invoke(this, EventArgs.Empty));
        public ICommand OneOneOneCommand => new Command(() => OneOneOneRequested?.Invoke(this, EventArgs.Empty));
    }
}
