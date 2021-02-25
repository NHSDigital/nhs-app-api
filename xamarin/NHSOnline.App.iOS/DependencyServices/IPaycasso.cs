using System.Threading.Tasks;

namespace NHSOnline.App.iOS.DependencyServices
{
    internal interface IPaycasso
    {
        Task<PaycassoCallbackResponse> Launch(PaycassoData data);
    }
}