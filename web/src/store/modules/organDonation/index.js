import { initialState } from './mutation-types';
import actions from './actions';
import getters from './getters';
import mutations from './mutations';

export default {
  namespaced: true,
  state: initialState,
  actions,
  getters,
  mutations,
};
