export default {
  biometricState(state) {
    return state.biometricsRegistrationStatus;
  },
  deviceBiometricType(state) {
    return state.biometricType;
  },
  getDeviceBiometricNameString(state) {
    if (state.biometricType === undefined) {
      return undefined;
    }
    return `loginSettings.biometrics.biometricType.${state.biometricType}`;
  },
  getBiometricInformation(state) {
    return `loginSettings.biometrics.warningText.${state.biometricType}`;
  },
  getBiometricWarningText(state) {
    return `loginSettings.biometrics.warningText.${state.biometricType}`;
  },

  retrieveError(state) {
    return state.errorCode;
  },

  retrieveCannotFindErrorText(state) {
    return `loginSettings.biometrics.errors.cannotFindBiometricType.errorText.${state.biometricType}`;
  },
};
