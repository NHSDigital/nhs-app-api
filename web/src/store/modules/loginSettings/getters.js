import biometricTypes from '@/lib/biometrics/biometricTypes';

export default {
  biometricSupported(state) {
    switch (state.biometricType) {
      case biometricTypes.TouchID:
      case biometricTypes.FaceID:
      case biometricTypes.Fingerprint:
      case biometricTypes.FingerprintFaceOrIris:
        return true;
      default:
        return false;
    }
  },
  biometricRegistered(state) {
    return state.biometricsRegistrationStatus;
  },
  isWaiting(state) {
    return state.isWaiting;
  },
  biometricType(state) {
    if (state.biometricType === undefined) {
      return biometricTypes.None;
    }
    return state.biometricType;
  },
  biometricError(state) {
    return state.errorCode;
  },
};
