import actions from './actions';
import getters from './getters';
import mutations from './mutations';

export default {
  namespaced: true,
  state() {
    return {
      durationSeconds: undefined,
      gpOdsCode: undefined,
      lastCalledAt: undefined,
      validationInterval: undefined,
    };
  },
  actions,
  getters,
  mutations,
};
