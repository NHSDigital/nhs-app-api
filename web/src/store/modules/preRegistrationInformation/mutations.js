
import moment from 'moment';
import { setCookie } from '@/lib/cookie-manager';
import { CONTINUE, SYNC } from './mutation-types';

export default {
  [CONTINUE](state) {
    state.seen = true;
    setCookie({
      cookies: this.app.$cookies,
      key: 'SkipPreRegistrationPage',
      value: true,
      options: {
        maxAge: moment.duration(5, 'y').asSeconds(),
        secure: this.app.$env.SECURE_COOKIES,
      },
    });
  },
  [SYNC](state) {
    const cookieState = !!this.app.$cookies.get('SkipPreRegistrationPage');
    state.seen = state.seen || !!cookieState;
    if (state.seen && !cookieState) {
      setCookie({
        cookies: this.app.$cookies,
        key: 'SkipPreRegistrationPage',
        value: state.seen,
        options: {
          maxAge: moment.duration(5, 'y').asSeconds(),
          secure: this.app.$env.SECURE_COOKIES,
        },
      });
    }
  },
};
