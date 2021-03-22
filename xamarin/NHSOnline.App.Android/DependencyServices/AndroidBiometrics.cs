using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBiometrics))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public sealed class AndroidBiometrics : IBiometrics
    {
        public Task<BiometricStatus> FetchBiometricStatus()
        {
            return Task.FromResult<BiometricStatus>(new BiometricStatus.HardwareNotPresent());
        }

        public Task<IBiometricAuthKey> CreateBiometricKey()
        {
            throw new System.NotImplementedException();
        }
        
        public bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key)
        {
            throw new System.NotImplementedException();
        }
    }
}