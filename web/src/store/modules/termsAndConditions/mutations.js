import { SET_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';

export default {
  [SET_ACCEPTANCE](state, result) {
    state.areAccepted = result;
  },
};
