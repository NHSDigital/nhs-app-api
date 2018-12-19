import { initialState } from './mutation-types';
import actions from './actions';
import mutations from './mutations';

export default {
  namespaced: true,
  state: initialState,
  actions,
  mutations,
};
