/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { setCookie } from '@/lib/cookie-manager';
import {
  DISMISS,
  SYNC,
  REFRESH_PAGE,
} from './mutation-types';
import { INDEX } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

const SECONDS_IN_A_DAY = 60 * 60 * 24;

export default {
  [DISMISS](state) {
    state.dismissed = true;
  },
  [REFRESH_PAGE](state) {
    redirectTo(this, INDEX.path, null);
  },
  [SYNC](state) {
    const cookieState = !!this.app.$cookies.get('HideBiometricBanner');
    state.dismissed = state.dismissed || cookieState;
    if (!cookieState) {
      setCookie({
        cookies: this.app.$cookies,
        key: 'HideBiometricBanner',
        value: state.dismissed,
        options: {
          secure: this.app.$env.SECURE_COOKIES,
          maxAge: ((this.app.$env.COOKIES_BANNER_EXPIRY_DAYS * 5) || 0) * SECONDS_IN_A_DAY,
        },
      });
    }
  },
};
