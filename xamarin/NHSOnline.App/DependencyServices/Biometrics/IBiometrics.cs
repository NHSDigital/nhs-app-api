using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometrics
    {
        Task<BiometricStatus> FetchBiometricStatus(string fidoUsername);

        Task<IBiometricAuthKey> CreateBiometricKey(string fidoUsername);
        bool TryGetKey(string fidoUsername, [NotNullWhen(true)] out IBiometricAuthKey? key);
    }
}
