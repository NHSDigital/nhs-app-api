import { mergeCookie } from '@/lib/cookie-manager';
import {
  SET_ACCEPTANCE,
  SET_UPDATED_CONSENT_REQUIRED,
  INIT_ACCEPTANCE,
} from '@/store/modules/termsAndConditions/mutation-types';

export default {
  [SET_ACCEPTANCE](state, result) {
    state.areAccepted = result.areAccepted;
    state.analyticsCookieAccepted = result.analyticsCookieAccepted;
    mergeCookie({
      cookies: this.$cookies,
      key: 'nhso.terms',
      value: {
        areAccepted: state.areAccepted,
        analyticsCookieAccepted: state.analyticsCookieAccepted,
      },
      secure: this.$env.SECURE_COOKIES,
    });
  },
  [SET_UPDATED_CONSENT_REQUIRED](state, result) {
    state.updatedConsentRequired = result;
    mergeCookie({
      cookies: this.$cookies,
      key: 'nhso.terms',
      value: { updatedConsentRequired: state.updatedConsentRequired },
      secure: this.$env.SECURE_COOKIES,
    });
  },
  [INIT_ACCEPTANCE](state) {
    state.areAccepted = false;
    state.updatedConsentRequired = false;
    state.analyticsCookieAccepted = false;
  },
};
