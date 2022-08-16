export default {
  thereIsAProblem: 'There is a problem',

  fingerPrintFaceOrIris: {
    title: 'Turn on fingerprint, face or iris recognition',
    text: 'You can log in with your fingerprint, face or iris instead of a password and security code.',
    warningText: 'All fingerprints, faces or irises registered on this device will be able to access connected health websites and apps that use your NHS login information.',
    doYouWantToTurnOn: 'Do you want to turn on fingerprint, face or iris recognition?',
    errorText: 'Choose if you want to turn on fingerprint, face or iris recognition',
    yes: 'Yes, turn on fingerprint, face or iris recognition',
    no: 'No, do not turn on fingerprint, face or iris recognition',
  },
  face: {
    title: 'Turn on Face ID',
    text: 'You can log in with your face instead of a password and security code.',
    warningText: 'All faces registered on this device will be able to access connected health websites and apps that use your NHS login information.',
    doYouWantToTurnOn: 'Do you want to turn on Face ID?',
    errorText: 'Choose if you want to turn on Face ID',
    yes: 'Yes, turn on Face ID',
    no: 'No, do not turn on Face ID',
  },
  touch: {
    title: 'Turn on Touch ID',
    text: 'You can log in with your fingerprint Instead of a password and security code.',
    warningText: 'All fingerprints registered on this device will be able to access connected health websites and apps that use your NHS login information.',
    doYouWantToTurnOn: 'Do you want to turn on Touch ID?',
    errorText: 'Choose if you want to turn on Touch ID',
    yes: 'Yes, turn on Touch ID',
    no: 'No, do not turn on Touch ID',
  },
  errors: {
    cannotFindBiometrics: {
      title: {
        fingerPrintFaceOrIris: 'fingerprint, face or iris (biometrics)',
        face: 'Face ID',
        touch: 'Touch ID',
      },
      checkYourDevice: 'Check your device settings to make sure you have:',
      turnOnBiometrics: {
        fingerPrintFaceOrIris: 'turned on biometrics',
        face: 'turned on Face ID',
        touch: 'turned on Touch ID',
      },
      addedBiometrics: {
        fingerPrintFaceOrIris: 'added a fingerprint, face or iris scan',
        face: 'added a face scan',
        touch: 'added a fingerprint scan',
      },
      ifYouCannotUse: {
        fingerPrintFaceOrIris: 'If you cannot use biometrics, you\'ll need to log in using your email, password and security code.',
        face: 'If you cannot use Face ID, you\'ll need to log in using your email, password and security code.',
        touch: 'If you cannot use Touch ID, you\'ll need to log in using your email, password and security code.',
      },
    },
    cannotTurnOnBiometrics: {
      title: {
        fingerPrintFaceOrIris: 'fingerprint, face or iris recognition (biometrics)',
        face: 'Face ID',
        touch: 'Touch ID',
      },
      technicalProblem: {
        face: 'We cannot turn on Face ID due to a technical problem.',
        touch: 'We cannot turn on Touch ID due to a technical problem.',
        fingerPrintFaceOrIris: 'We cannot turn on fingerprint, face or iris recognition (biometrics) due to a technical problem.',
      },
      tryAgainLater: 'Try again later.',
      ifYouStillCannotTurnOn: {
        face: 'If you still cannot turn on Face ID, ',
        touch: 'If you still cannot turn on Touch ID, ',
        fingerPrintFaceOrIris: 'If you still cannot turn on biometrics, ',
      },
      getHelpWithLoggingIn: 'get help with logging in using biometrics',
      loginUsingOtherMeans: {
        face: 'If you cannot turn on Face ID, you\'ll need to log in using your email, password and security code.',
        touch: 'If you cannot turn on Touch ID, you\'ll need to log in using your email, password and security code.',
        fingerPrintFaceOrIris: 'If you cannot turn on biometrics, you\'ll need to log in using your email, password and security code.',
      },
    },
  },

};
