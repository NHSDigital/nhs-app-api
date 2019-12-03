import get from 'lodash/fp/get';
import {
  INIT,
  LOADED,
  SET_SUMMARIES,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  load({ commit }, clearApiError) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');
      commit(LOADED, false);
    }
    return this.app.$http.getV1PatientMessages()
      .then((response) => {
        commit(SET_SUMMARIES, get('messageSummaries', response));
        commit(LOADED, true);
      }).catch(() => {});
  },
  clearErrorsAndLoad() {
    return this.dispatch('patientPracticeMessaging/load', true);
  },
};
