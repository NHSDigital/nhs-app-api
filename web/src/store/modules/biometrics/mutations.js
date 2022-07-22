import {
  SET_BIOMETRICS_COOKIE_EXISTS,
  SHOW_BIOMETRIC_SPINNER,
} from './mutation-types';

export default {
  [SET_BIOMETRICS_COOKIE_EXISTS](state, exists) {
    state.biometricsCookieExists = exists;
  },
  [SHOW_BIOMETRIC_SPINNER](state, show) {
    state.showBiometricSpinner = show;
  },
};
