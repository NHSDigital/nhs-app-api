import actions from './actions';
import mutations from './mutations';
import getters from './getters';

export default {
  namespaced: true,
  state() {
    return {
      showApiError: true,
      apiErrors: [],
      apiErrorButtonPath: '',
      hasConnectionProblem: false,
    };
  },
  actions,
  mutations,
  getters,
};
