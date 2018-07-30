import actions from './actions';
import getters from './getters';
import mutations from './mutations';
import { initialState } from './mutation-types';

export default {
  namespaced: true,
  state() {
    return initialState;
  },
  actions,
  getters,
  mutations,
};
