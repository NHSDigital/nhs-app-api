using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometrics
    {
        Task<BiometricStatus?> FetchBiometricStatus();
    }
}
