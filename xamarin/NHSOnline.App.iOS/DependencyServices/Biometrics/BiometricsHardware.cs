using Foundation;
using LocalAuthentication;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Logging;

namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    internal sealed class BiometricsHardware
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<BiometricsHardware>();

        internal static bool HasBiometricHardware(LAContext context, out BiometricHardwareState state)
        {
            NSError? error = null;
            try
            {
                var enabled = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);
                if (enabled)
                {
                    state = BiometricHardwareState.Usable;
                    return true;
                }

                Logger.LogInformation("CanEvaluatePolicy returned error: {Error}", error);

                state = BiometricHardwareState.Unusable;

                return (LAStatus?)(long?)error?.Code switch
                {
                    LAStatus.BiometryNotEnrolled => true,
                    LAStatus.BiometryLockout => true,
                    LAStatus.BiometryNotAvailable => false,
                    _ => false
                };
            }
            finally
            {
                error?.Dispose();
            }
        }
    }
}