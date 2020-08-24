import { setCookie } from '@/lib/cookie-manager';
import { CONTINUE, SYNC } from './mutation-types';

export default {
  [CONTINUE](state) {
    state.seen = true;
    setCookie({
      cookies: this.$cookies,
      key: 'SkipPreRegistrationPage',
      value: true,
      expires: '5y',
      secure: this.$env.SECURE_COOKIES,
    });
  },
  [SYNC](state) {
    const cookieState = !!this.$cookies.get('SkipPreRegistrationPage');
    state.seen = state.seen || !!cookieState;
    if (state.seen && !cookieState) {
      setCookie({
        cookies: this.$cookies,
        key: 'SkipPreRegistrationPage',
        value: state.seen,
        expires: '5y',
        secure: this.$env.SECURE_COOKIES,
      });
    }
  },
};
