import {
  REFERRALS_LOADED,
  UPCOMING_APPOINTMENTS_LOADED,
} from './mutation-types';


export default {
  async load({ commit }) {
    const { referrals, upcomingAppointments }
       = await this.app.$http.getV1PatientSecondaryCareSummary({});

    commit(REFERRALS_LOADED, referrals);
    commit(UPCOMING_APPOINTMENTS_LOADED, upcomingAppointments);
  },
};
