import jwt from 'jwt-decode';
import { setCookie } from '@/lib/cookie-manager';
import {
  SET_BIOMETRICS_COOKIE_EXISTS,
  SHOW_BIOMETRIC_SPINNER,
} from './mutation-types';

export default {
  addBiometricsCookie() {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);

    const cookieExists = !!this.$cookies.get(`nhso.biometrics-prompt-${decodedToken.sub}`);
    if (!cookieExists) {
      setCookie({
        cookies: this.$cookies,
        key: `nhso.biometrics-prompt-${decodedToken.sub}`,
        value: decodedToken.sub,
        expires: '1y',
        secure: this.$env.SECURE_COOKIES,
      });
    }
  },
  checkBiometricsCookie({ commit }) {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);

    const cookieExists = !!this.$cookies.get(`nhso.biometrics-prompt-${decodedToken.sub}`);

    commit(SET_BIOMETRICS_COOKIE_EXISTS, cookieExists);
  },
  showBiometricSpinner({ commit }, showSpinner) {
    commit(SHOW_BIOMETRIC_SPINNER, showSpinner);
  },
};
