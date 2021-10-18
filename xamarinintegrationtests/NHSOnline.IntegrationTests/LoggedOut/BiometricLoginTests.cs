using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-04.1", "Launching the app when Android fingerprint biometrics has been enabled displays a prompt")]
    [BusinessRule("BR-LOG-04.2", "Successful authentication of the user's Android fingerprint logs the user in")]
    [BusinessRule("BR-LOG-04.3", "Unsuccessful authentication of the user's Android fingerprint shows an error")]
    [BusinessRule("BR-LOG-04.5", "Launching the app when iOS Touch ID biometrics has been enabled displays a prompt")]
    [BusinessRule("BR-LOG-04.6" ,"Unsuccessful authentication of Touch ID when a fingerprint cannot be found displays an error message")]
    [BusinessRule("BR-LOG-04.7" ,"Launching the app when Face ID biometrics has been enabled displays a prompt")]
    [BusinessRule("BR-LOG-04.8" ,"Successful authentication of the user's face logs the user in")]
    [BusinessRule("BR-LOG-04.9" ,"Unsuccessful authentication of the user's Face ID displays an error message")]
    [BusinessRule("BR-LOG-04.10","Cancelling the biometrics log in prompt displays the logged out home screen")]
    [BusinessRule("BR-LOG-04.11","Successful authentication of the user's Touch ID logs the user in")]
    [BusinessRule("BR-LOG-04.12","Consecutive unsuccessful attempts to authenticate with Touch ID displays a message")]
    [BusinessRule("BR-LOG-04.13","Launching the app when Touch ID was enabled in the app but disabled in the device settings displays an error message")]
    [BusinessRule("BR-LOG-04.14","Successful login with credentials after biometrics has been disabled due to repeated failed attempts with Touch ID allows biometrics to be enabled again")]
    [BusinessRule("BR-LOG-04.16","Launching the app when Face ID was enabled in the app but disabled in the device settings displays an error message")]
    [BusinessRule("BR-LOG-04.18","Changing any biometrics data registered on the device when biometrics is enabled in the app displays a message on launch of the app")]
    [BusinessRule("BR-LOG-04.19","Navigating to the login screen when biometrics have been disabled by repeated unsuccessful attempts does not show a prompt to log in with biometrics")]
    [BusinessRule("BR-LOG-04.20","Unsuccessful authentication with Touch ID due to an error from NHS login displays an error message")]
    [BusinessRule("BR-LOG-04.21","Unsuccessful authentication with Face ID due to an error from NHS login displays an error message")]
    [BusinessRule("BR-LOG-04.22","Unsuccessful authentication with Fingerprint due to an error from NHS login displays an error message")]
    [BusinessRule("BR-LOG-04.23","Backgrounding the app when the Face ID prompt is displayed on login displays an error when the user returns to the app")]
    [BusinessRule("BR-LOG-04.24","Backgrounding the app when the Touch ID prompt is displayed on login displays an error when the user returns to the app")]
    [BusinessRule("BR-LOG-04.25","Backgrounding the app when the biometric prompt is displayed on login displays an error when the user returns to the app")]
    [BusinessRule("BR-LOG-04.26","Consecutive unsuccessful attempts to authenticate with Fingerprint ID displays a message")]
    [BusinessRule("BR-LOG-04.27","Returning to the logged out home screen after receiving unsuccessful biometric error message authentication displays the biometric prompt again")]
    [BusinessRule("BR-LOG-04.30","If biometric authentication was turned on in (legacy) NHS App, but the device fingerprint  sensor does not meet new security requirements, after upgrade (migration) on first NHS APP launch a shutter page is displayed (concerns Android)")]
    [BusinessRule("BR-LOG-04.31","If biometric authentication was turned on in (legacy) NHS App, but the device fingerprint  sensor does not meet new security requirements, after upgrade (migration) User needs to login using credentials and try using another biometric method (concerns Android)")]
    public class BiometricLoginTests
    {
        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientWithAndroidFingerpintRegisteredDisplaysPromptOnLogin() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientLoggingInWithAndroidFingerprintIsSuccessfullyLoggedIn() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientLoggingInWithAndroidFingerprintUnsuccessfullyIsShownAnError() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientWithIosTouchIdRegisteredDisplaysPromptOnLogin() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientLoggingInWithIosTouchIdUnsuccessfullyIsShownAnError() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientWithIosFaceIdRegisteredDisplaysPromptOnLogin() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientWithIosFaceIdIsAbleToLogInSuccessfully() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientLoggingInWithIosFaceIdUnsuccessfullyIsShownAnError() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientCancellingBiometricsPromptIsShownTheLoggedOutHomeScreenAndroidFingerprint() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientCancellingBiometricsPromptIsShownTheLoggedOutHomeScreenIosTouchId() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientCancellingBiometricsPromptIsShownTheLoggedOutHomeScreenIosFaceId() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientWithIosTouchIdIsAbleToLogInSuccessfully() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLogInMultipleTimesUnsuccessfullyIsShownAnErrorTouchId() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientWithTouchIdRegisteredButDisabledOnTheDeviceIsShownAAnError() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientWithIsAbleToLogInSuccessfullyAndEnableBiometricsAfterMultipleUnsuccessfulAttemptsToLogInWithTouchId() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientWithFaceIdRegisteredButDisabledOnTheDeviceIsShownAAnError() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientAddingANewAndroidFingerprintOnTheDeviceIsShownAnError() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientAddingANewIosTouchIdOnTheDeviceIsShownAnError() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientAddingANewIosFaceIdOnTheDeviceIsShownAnError() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLogInWithAndroidFingerprintIdMultipleTimesIsNotShownAPromptToLogInWithAndroidFingerprint() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLogInWithIosTouchIdIdMultipleTimesIsNotShownAPromptToLogInWithIosTouchId() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLoginWithIosTouchIdThatEncountersAnErrorFromNhsLoginIsShownAnError() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLoginWithIosFaceIdThatEncountersAnErrorFromNhsLoginIsShownAnError() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLoginWithAndroidFingerprintThatEncountersAnErrorFromNhsLoginIsShownAnError() { }

        [NhsAppManualTest("NHSO-14303", "Unable to automate biometric tests at the moment")]
        public void APatientBackgroundingTheAppWhileTheLoginPromptIsDisplayedIsShownAnErrorIosFaceId() { }

        [NhsAppManualTest("NHSO-14299", "Unable to automate biometric tests at the moment")]
        public void APatientBackgroundingTheAppWhileTheLoginPromptIsDisplayedIsShownAnErrorIosTouchId() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientBackgroundingTheAppWhileTheLoginPromptIsDisplayedIsShownAnErrorAndroidFingerprint() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientAttemptingToLogInMultipleTimesUnsuccessfullyIsShownAnErrorAndroidFingerprint() { }

        [NhsAppManualTest("NHSO-14305", "Unable to automate biometric tests at the moment")]
        public void APatientReturningToTheLoggedOutHomeScreenAfterMultipleUnsuccessfulAttemptsIsShownTheBiometricPromptAndroidFingerprint() { }

        [NhsAppManualTest("NHSO-17076", "Unable to automate biometric tests at the moment")]
        public void APatientWhoHasLoggedInToLegacyAppWithFingerprintReceivesShutterScreenOnlyOnceOnXamarinWhenTheirPhoneSensorIsDeemedInvalidAndroid() { }
    }
}