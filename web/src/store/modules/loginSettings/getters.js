export default {
  biometricState(state) {
    return state.biometricsRegistrationStatus;
  },
  deviceBiometricType(state) {
    return state.biometricType;
  },
  getBiometricInformation(state) {
    switch (state.biometricType) {
      case 'loginSettings.biometrics.biometricType.fingerPrint':
        return 'loginSettings.biometrics.biometricInformation.fingerPrint';
      case 'loginSettings.biometrics.biometricType.face':
        return 'loginSettings.biometrics.biometricInformation.face';
      case 'loginSettings.biometrics.biometricType.touch':
        return 'loginSettings.biometrics.biometricInformation.touch';
      default:
        return undefined;
    }
  },
  getBiometricWarningText(state) {
    switch (state.biometricType) {
      case 'loginSettings.biometrics.biometricType.fingerPrint':
        return 'loginSettings.biometrics.warningText.fingerPrint';
      case 'loginSettings.biometrics.biometricType.face':
        return 'loginSettings.biometrics.warningText.face';
      case 'loginSettings.biometrics.biometricType.touch':
        return 'loginSettings.biometrics.warningText.touch';
      default:
        return undefined;
    }
  },
};
