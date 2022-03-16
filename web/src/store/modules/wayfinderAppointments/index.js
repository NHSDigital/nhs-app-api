import actions from './actions';
import mutations from './mutations';
import getters from './getters';
import { initialState } from './mutation-types';

export default {
  namespaced: true,
  state: initialState,
  actions,
  mutations,
  getters,
};
