import { SET_WAITING,
  UPDATE_REGISTRATION_STATUS,
  UPDATE_BIOMETRIC_TYPE,
  ADD_ERROR_CODE,
  CLEAR_ERROR_CODE } from './mutation-types';
import biometricTypes from '@/lib/biometrics/biometricTypes';

export default {
  [UPDATE_REGISTRATION_STATUS](state, biometricRegistered) {
    state.biometricsRegistrationStatus = biometricRegistered;
  },

  [SET_WAITING](state, isWaiting) {
    state.isWaiting = isWaiting;
  },

  [UPDATE_BIOMETRIC_TYPE](state, biometricType) {
    switch (biometricType) {
      case biometricTypes.TouchID:
        state.biometricLocaleReference =
          `loginSettings.biometrics.biometricType.${biometricTypes.TouchID}`;
        state.biometricType = biometricTypes.TouchID;
        break;
      case biometricTypes.FaceID:
        state.biometricLocaleReference =
          `loginSettings.biometrics.biometricType.${biometricTypes.FaceID}`;
        state.biometricType = biometricTypes.FaceID;
        break;
      case biometricTypes.Fingerprint:
        state.biometricLocaleReference =
          `loginSettings.biometrics.biometricType.header.${biometricTypes.Fingerprint}`;
        state.biometricType = biometricTypes.Fingerprint;
        break;
      default:
        state.biometricLocaleReference = undefined;
        state.biometricType = undefined;
        break;
    }
  },

  [ADD_ERROR_CODE](state, errorCode) {
    state.errorCode = errorCode;
  },

  [CLEAR_ERROR_CODE](state) {
    state.errorCode = undefined;
  },
};
