
import moment from 'moment';
import { setCookie } from '@/lib/cookie-manager';
import { DISMISS, SYNC } from './mutation-types';

export default {
  [DISMISS](state) {
    state.dismissed = true;
  },
  [SYNC](state) {
    const cookieState = !!this.$cookies.get('HideBiometricBanner');
    state.dismissed = state.dismissed || !!cookieState;
    if (state.dismissed && !cookieState) {
      setCookie({
        cookies: this.$cookies,
        key: 'HideBiometricBanner',
        value: state.dismissed,
        options: {
          maxAge: moment.duration(5, 'y').asSeconds(),
          secure: this.$env.SECURE_COOKIES,
        },
      });
    }
  },
};
