import actions from './actions';
import mutations from './mutations';

export default {
  namespaced: true,
  state() {
    return {
      webVersion: '',
      nativeVersion: '',
      platform: 'web',
    };
  },
  actions,
  mutations,
};
