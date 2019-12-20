import { initialState } from '@/store/modules/myRecord/mutation-types';
import actions from '@/store/modules/myRecord/actions';
import mutations from '@/store/modules/myRecord/mutations';
import getters from '@/store/modules/myRecord/getters';

export default {
  namespaced: true,
  state: initialState,
  actions,
  mutations,
  getters,
};
