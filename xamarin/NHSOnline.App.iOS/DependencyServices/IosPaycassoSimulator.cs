using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using NHSOnline.App.DependencyServices.Paycasso;

#if SIMULATOR
[assembly: Dependency(typeof(IosPaycassoSimulator))]
#endif
namespace NHSOnline.App.iOS.DependencyServices
{
    internal sealed class IosPaycassoSimulator: IPaycasso
    {
        public Task<PaycassoResult> Launch(LaunchPaycassoRequest request)
        {
            PaycassoResult paycassoFailedResult = new PaycassoResult.Failure("Paycasso not supported on Simulator");
            return Task.FromResult(paycassoFailedResult);
        }
    }
}