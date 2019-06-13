import actions from './actions';
import getters from './getters';
import mutations from './mutations';
import { initialState } from './mutation-types';

export default {
  actions,
  mutations,
  getters,
  namespaced: true,
  state: initialState,
};
