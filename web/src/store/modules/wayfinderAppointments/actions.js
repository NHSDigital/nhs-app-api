import {
  REFERRALS_LOADED,
  UPCOMING_APPOINTMENTS_LOADED,
  PAST_APPOINTMENTS_LOADED,
} from './mutation-types';


export default {
  async load({ commit }) {
    // Ignoring response from API call for now
    await this.app.$http.getV1PatientSecondaryCareSummary();

    commit(REFERRALS_LOADED, []);
    commit(UPCOMING_APPOINTMENTS_LOADED, []);
    commit(PAST_APPOINTMENTS_LOADED, []);
  },
};
