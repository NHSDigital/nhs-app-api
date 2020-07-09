export default {
  biometrics: {
    biometricInformation: {
      fingerPrint: 'You can log in with your fingerprint instead of a password and security code.',
      face: 'Face ID lets you log in with your face scan instead of a password and security code.',
      touch: 'Touch ID lets you log in with your fingerprint instead of a password and security code.',
    },
    biometricType: {
      fingerPrint: 'Fingerprint',
      face: 'Face ID',
      touch: 'Touch ID',
    },
    toggleLabel: {
      fingerPrint: 'Log in with fingerprint',
      face: 'Log in with Face ID',
      touch: 'Log in with Touch ID',
    },
    warningText: {
      fingerPrint: 'All fingerprints registered on this device will be able to access connected health websites and apps that use your NHS login information.',
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
      cannotFindBiometricType: {
        errorText: {
          face: 'Check that you have added a face scan in your device\'s Face ID settings.',
          touch: 'Check that you have added a fingerprint in your device\'s Touch ID settings.',
          fingerPrint: 'Check that you have added a fingerprint in your device\'s security settings.',
        },
      },
      cannotChangeBiometricSettings: {
        paragraph1: 'Go back and try again.',
        paragraph2: 'If you keep seeing this message, return to your settings later.',
      },
    },
  },
};
