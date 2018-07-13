import actions from './actions';
import mutations from './mutations';

export default {
  namespaced: true,
  state() {
    return {
      message: '',
      hasBeenShown: false,
      show: false,
      type: 'success',
    };
  },
  actions,
  mutations,
};
