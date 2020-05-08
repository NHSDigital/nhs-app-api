import actions from './actions';
import mutations from './mutations';
import { initialState } from './mutation-types';
import getters from './getters';

export default {
  namespaced: true,
  state: initialState,
  actions,
  getters,
  mutations,
};
