using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using LocalAuthentication;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.iOS.DependencyServices.Biometrics;
using UIKit;
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

        // NHSO-14610: Will need to use this property to correctly show the current users biometrics state.
        public string BiometricsUsername { get; set; } = string.Empty;

        public Task<BiometricStatus> FetchBiometricStatus()
        {
            BiometricStatus status = new BiometricStatus.HardwareNotPresent();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                using var context = new LAContext();
                if (BiometricsHardware.HasBiometricHardware(context, out var state))
                {
                    var registrationStatus = DeriveRegistrationStatus(context);
                    if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                    {
                        status = context.BiometryType switch
                        {
                            LABiometryType.FaceId => new BiometricStatus.FaceId(state, registrationStatus),
                            _ => new BiometricStatus.TouchId(state, registrationStatus)
                        };
                    }
                    else
                    {
                        status = new BiometricStatus.TouchId(state, registrationStatus);
                    }
                }
            }

            return Task.FromResult(status);
        }

        public Task<IBiometricAuthKey> CreateBiometricKey() => _biometricAuthKeyProvider.CreateBiometricKey();

        public bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key) => _biometricAuthKeyProvider.TryGetKey(out key);

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