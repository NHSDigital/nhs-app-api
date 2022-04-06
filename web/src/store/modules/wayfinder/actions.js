import { createLocalError } from '@/lib/utils';
import {
  INIT,
  REFERRALS_LOADED,
  UPCOMING_APPOINTMENTS_LOADED,
  SHOW_ERROR,
  HAS_LOADED,
} from './mutation-types';


export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    try {
      const { referrals, upcomingAppointments }
        = await this.app.$http.getV1PatientSecondaryCareSummary({ ignoreError: true });

      commit(REFERRALS_LOADED, referrals);
      commit(UPCOMING_APPOINTMENTS_LOADED, upcomingAppointments);
    } catch (error) {
      const apiError = createLocalError(error);

      if (apiError.status === 504 || apiError.status === 502) {
        commit(SHOW_ERROR, apiError);
      } else {
        this.dispatch('errors/addApiError', error);
      }
    } finally {
      commit(HAS_LOADED, true);
    }
  },
};
