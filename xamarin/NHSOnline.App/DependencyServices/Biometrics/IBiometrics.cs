using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometrics
    {
        string BiometricsUsername { get; set; }
        Task<BiometricStatus> FetchBiometricStatus();

        Task<IBiometricAuthKey> CreateBiometricKey();
        bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key);
    }
}
