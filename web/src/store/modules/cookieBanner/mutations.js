/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { setCookie } from '@/lib/cookie-manager';
import {
  ACKNOWLEDGE,
  SYNC,
} from './mutation-types';

const SECONDS_IN_A_DAY = 60 * 60 * 24;

export default {
  [ACKNOWLEDGE](state) {
    state.acknowledged = true;
    setCookie({
      cookies: this.app.$cookies,
      key: 'nhso.cookie_options',
      value: state.acknowledged,
      options: {
        secure: this.app.$env.SECURE_COOKIES,
        maxAge: (this.app.$env.COOKIES_BANNER_EXPIRY_DAYS || 0) * SECONDS_IN_A_DAY,
      },
    });
  },
  [SYNC](state) {
    const cookieState = !!this.app.$cookies.get('nhso.cookie_options');
    state.acknowledged = state.acknowledged || cookieState;
    if (!cookieState) {
      setCookie({
        cookies: this.app.$cookies,
        key: 'nhso.cookie_options',
        value: state.acknowledged,
        options: {
          secure: this.app.$env.SECURE_COOKIES,
          maxAge: (this.app.$env.COOKIES_BANNER_EXPIRY_DAYS || 0) * SECONDS_IN_A_DAY,
        },
      });
    }
  },
};
