using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;

namespace NHSOnline.App.DependencyServices.Paycasso
{
    public interface IPaycasso
    {
        Task<PaycassoResult> Launch(LaunchPaycassoRequest request);
    }
}