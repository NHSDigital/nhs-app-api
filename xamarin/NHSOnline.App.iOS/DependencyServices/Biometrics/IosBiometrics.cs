using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using LocalAuthentication;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.iOS.DependencyServices.Biometrics;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosBiometrics))]
namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    public sealed class IosBiometrics : IBiometrics
    {
        private readonly IBiometricAuthKeyProvider _biometricAuthKeyProvider;

        public IosBiometrics()
        {
#if SIMULATOR
            _biometricAuthKeyProvider = new SimulatedBiometricAuthKeyProvider();
#endif
#if IPHONE
            _biometricAuthKeyProvider = new BiometricAuthKeyProvider();
#endif
        }

        public Task<BiometricStatus> FetchBiometricStatus(string fidoUsername)
        {
            BiometricStatus status = new BiometricStatus.HardwareNotPresent();

            using var context = new LAContext();
            if (BiometricsHardware.HasDeviceOwnerPermittedUseOfBiometricHardware(context, out var state))
            {
                var registrationStatus = DeriveRegistrationStatus(context);
                status = context.BiometryType switch
                {
                    LABiometryType.FaceId => new BiometricStatus.FaceId(state, registrationStatus),
                    _ => new BiometricStatus.TouchId(state, registrationStatus)
                };
            }
            else
            {
                status = context.BiometryType switch
                {
                    LABiometryType.FaceId => new BiometricStatus.FaceId(state, BiometricRegistrationStatus.NotRegistered),
                    _ => new BiometricStatus.TouchId(state, BiometricRegistrationStatus.NotRegistered)
                };
            }

            return Task.FromResult(status);
        }

        public Task<IBiometricAuthKey> CreateBiometricKey(string fidoUsername) => _biometricAuthKeyProvider.CreateBiometricKey();

        public bool TryGetKey(string fidoUsername, [NotNullWhen(true)] out IBiometricAuthKey? key) => _biometricAuthKeyProvider.TryGetKey(out key);

        public bool IsPermissionBased()
        {
            using var context = new LAContext();
            return context.BiometryType switch
            {
                LABiometryType.FaceId => true,
                _=> false
            };
        }

        private static BiometricRegistrationStatus DeriveRegistrationStatus(LAContext context)
        {
            using var registeredDomainState = BiometricRegistrationDomainState.Get();

            if (registeredDomainState is null)
            {
                return BiometricRegistrationStatus.NotRegistered;
            }

            if (registeredDomainState.Equals(context.EvaluatedPolicyDomainState))
            {
                return BiometricRegistrationStatus.Registered;
            }

            return BiometricRegistrationStatus.Invalidated;
        }
    }
}