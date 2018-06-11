/* eslint-disable object-shorthand */
/* eslint-disable func-names */
import actions from './actions';
import mutations from './mutations';
import getters from './getters';

export default {
  namespaced: true,
  state() {
    return {
      showingApiErrorCondition: function (status) { return status >= 500; },
      apiErrors: [],
      apiErrorButtonPath: '',
      hasConnectionProblem: false,
    };
  },
  actions,
  mutations,
  getters,
};
