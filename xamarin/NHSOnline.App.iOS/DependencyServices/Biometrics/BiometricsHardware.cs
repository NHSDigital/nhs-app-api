using System;
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

        internal static bool HasDeviceOwnerPermittedUseOfBiometricHardware(LAContext context, out BiometricHardwareState state, out bool enrolled)
        {
            NSError? error = null;
            try
            {
                var enabled = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);
                if (enabled)
                {
                    state = BiometricHardwareState.Usable;
                    enrolled = true;
                    return true;
                }

                Logger.LogInformation("CanEvaluatePolicy returned error: {Error}", error);

                state = BiometricHardwareState.Unusable;

                enrolled = (LAStatus?)(long?)error?.Code switch
                {
                    LAStatus.BiometryNotEnrolled => false,
                    LAStatus.BiometryLockout => false,
                    _ => true
                };
                return enrolled;
            }
            finally
            {
                error?.Dispose();
            }
        }
    }
}