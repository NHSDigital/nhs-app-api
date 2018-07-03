/* eslint-disable import/extensions */
import ErrorSettings from '@/components/errors/Settings';
import actions from './actions';
import mutations from './mutations';
import getters from './getters';

export default {
  namespaced: true,
  state() {
    return {
      showApiError: true,
      apiErrors: [],
      pageSettings: ErrorSettings.default,
      hasConnectionProblem: false,
      routePath: '',
    };
  },
  actions,
  mutations,
  getters,
};
