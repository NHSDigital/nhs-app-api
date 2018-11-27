import { SET_ACCEPTANCE, SET_UPDATED_CONSENT_REQUIRED, INIT_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';

export default {
  [SET_ACCEPTANCE](state, result) {
    state.areAccepted = result;
    if (result) {
      state.updatedConsentRequired = false;
    }
  },
  [SET_UPDATED_CONSENT_REQUIRED](state, result) {
    state.updatedConsentRequired = result;
  },
  [INIT_ACCEPTANCE](state) {
    state.areAccepted = false;
    state.updatedConsentRequired = false;
  },
};
