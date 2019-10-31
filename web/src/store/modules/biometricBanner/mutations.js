
import moment from 'moment';
import { setCookie } from '@/lib/cookie-manager';
import { DISMISS, SYNC } from './mutation-types';

export default {
  [DISMISS](state) {
    state.dismissed = true;
    setCookie({
      cookies: this.app.$cookies,
      key: 'HideBiometricBanner',
      value: true,
      options: {
        maxAge: moment.duration(5, 'y').asSeconds(),
        secure: this.app.$env.SECURE_COOKIES,
      },
    });
  },
  [SYNC](state) {
    const cookieState = !!this.app.$cookies.get('HideBiometricBanner');
    state.dismissed = state.dismissed || !!cookieState;
    if (state.dismissed && !cookieState) {
      setCookie({
        cookies: this.app.$cookies,
        key: 'HideBiometricBanner',
        value: state.dismissed,
        options: {
          maxAge: moment.duration(5, 'y').asSeconds(),
          secure: this.app.$env.SECURE_COOKIES,
        },
      });
    }
  },
};
