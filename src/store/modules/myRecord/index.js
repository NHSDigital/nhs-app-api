import actions from './actions';
import mutations from './mutations';
import getters from './getters';

export default {
  namespaced: true,
  state() {
    return {
      allergies: [],
      allergiesHasLoaded: false,
      demographicsHasLoaded: false,
      patientDemographics: null,
    };
  },
  actions,
  mutations,
  getters,
};
