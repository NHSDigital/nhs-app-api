export default {
  biometrics: {
    biometricInformation: {
      fingerPrintFaceOrIris: 'You can log in with your fingerprint, face or iris instead of a password and security code if your device meets Google\'s increased security settings.',
      face: 'Face ID lets you log in with your face scan instead of a password and security code.',
      touch: 'Touch ID lets you log in with your fingerprint instead of a password and security code.',
    },
    biometricType: {
      fingerPrint: 'Fingerprint',
      fingerPrintFaceOrIris: 'Fingerprint, face or iris',
      face: 'Face ID',
      touch: 'Touch ID',
    },
    toggleLabel: {
      fingerPrint: 'Log in with fingerprint',
      fingerPrintFaceOrIris: 'Log in with fingerprint, face or iris',
      face: 'Log in with Face ID',
      touch: 'Log in with Touch ID',
    },
    warningText: {
      fingerPrint: 'All fingerprints registered on this device will be able to access connected health websites and apps that use your NHS login information.',
      fingerPrintFaceOrIris: 'All fingerprints, faces or irises registered on this device will be able to access connected health websites and apps that use your NHS login information.',
      face: 'All Face IDs registered on this device will be able to access connected health websites and apps that use your NHS login information.',
      touch: 'All fingerprints registered on this device will be able to access connected health websites and apps that use your NHS login information.',
    },
    noBiometricType: {
      settingsLinkText: 'Login options',
      information: {
        paragraph1: 'We cannot find any fingerprint or face recognition settings on your device.',
        paragraph2: 'You\'ll need to continue using your password and security code to log in.',
      },
    },
    errors: {
      title: {
        fingerPrint: 'fingerprint',
        fingerPrintFaceOrIris: 'fingerprint, face or iris',
        face: 'Face ID',
        touch: 'Touch ID',
      },
      cannotFindBiometricType: {
        errorText: {
          face: 'Check that you have added a face scan in your device\'s Face ID settings.',
          touch: 'Check that you have added a fingerprint in your device\'s Touch ID settings.',
          fingerPrintFaceOrIris: 'Check that you have added a fingerprint, face or iris in your device\'s security settings.',
        },
        weCannotSupport: 'We cannot support fingerprint, face or iris recognition (biometrics) on Android devices with sensors that do not meet Google\'s increased security settings.',
        getHelp: 'Get help with logging in using biometrics',
        ifYouCantUseBiometrics: 'If you cannot use biometrics, you\'ll need to log in using your email, password and security code.',
      },
      cannotChangeBiometricSettings: {
        paragraph1: 'Go back and try again.',
        paragraph2: 'If you keep seeing this message, return to your settings later.',
      },
    },
  },
};
