import { SET_ACCEPTANCE, INIT_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';

export default {
  [SET_ACCEPTANCE](state, result) {
    state.areAccepted = result;
  },
  [INIT_ACCEPTANCE](state) {
    state.areAccepted = false;
  },
};
