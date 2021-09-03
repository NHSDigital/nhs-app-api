import biometricTypes from '@/lib/biometrics/biometricTypes';
import {
  SET_WAITING,
  UPDATE_REGISTRATION_STATUS,
  UPDATE_BIOMETRIC_TYPE,
  ADD_ERROR_CODE,
  CLEAR_ERROR_CODE } from './mutation-types';

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
        state.biometricType = biometricTypes.TouchID;
        break;
      case biometricTypes.FaceID:
        state.biometricType = biometricTypes.FaceID;
        break;
      case biometricTypes.Fingerprint:
        state.biometricType = biometricTypes.Fingerprint;
        break;
      case biometricTypes.FingerprintFaceOrIris:
        state.biometricType = biometricTypes.FingerprintFaceOrIris;
        break;
      default:
        state.biometricType = biometricTypes.None;
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
