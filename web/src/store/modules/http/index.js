import actions from './actions';
import mutations from './mutations';

export default {
  namespaced: true,
  state() {
    return {
      isLoading: false,
      cancelRequestHandlers: [],
    };
  },
  actions,
  mutations,
};
