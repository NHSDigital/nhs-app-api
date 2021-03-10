using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using NHSOnline.App.DependencyServices.Paycasso;
using NHSOnline.App.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidPaycasso))]
namespace NHSOnline.App.Droid.DependencyServices
{
    internal sealed class AndroidPaycasso: IPaycasso
    {
        public Task<PaycassoResult> Launch(LaunchPaycassoRequest request)
        {
            throw new NotImplementedException("Not implemented on Android");
        }
    }
}