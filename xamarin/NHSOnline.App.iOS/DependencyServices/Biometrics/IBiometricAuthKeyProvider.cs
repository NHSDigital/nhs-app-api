using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;

namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    public interface IBiometricAuthKeyProvider
    {
        Task<IBiometricAuthKey> CreateBiometricKey();
        bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key);
    }
}