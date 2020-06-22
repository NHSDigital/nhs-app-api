using System.ComponentModel;
using NHSOnline.App.Presenters;

namespace NHSOnline.App.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage: IMainView
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
