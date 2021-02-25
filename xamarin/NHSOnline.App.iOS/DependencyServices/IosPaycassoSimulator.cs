#if SIMULATOR
using System.Threading.Tasks;

namespace NHSOnline.App.iOS.DependencyServices
{
    internal sealed class IosPaycassoSimulator: IPaycasso
    {
        public Task<PaycassoCallbackResponse> Launch(PaycassoData data)
        {
            return Task.FromResult(PaycassoCallbackResponse.ForError("Paycasso not supported on Simulator"));
        }
    }
}
#endif