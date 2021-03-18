using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosBiometrics))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosBiometrics : IBiometrics
    {
        public Task<BiometricSpec?> FetchBiometricSpec()
        {
            return Task.FromResult<BiometricSpec?>(BiometricSpec.Face(true));
        }
    }
}