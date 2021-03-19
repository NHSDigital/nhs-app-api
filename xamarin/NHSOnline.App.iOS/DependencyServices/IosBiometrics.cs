using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosBiometrics))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosBiometrics : IBiometrics
    {
        public Task<BiometricStatus?> FetchBiometricStatus()
        {
            return Task.FromResult<BiometricStatus?>(BiometricStatus.Face(false));
        }
    }
}