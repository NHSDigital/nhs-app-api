import { createLocalError } from '@/lib/utils';
import {
  INIT,
  CONFIRMED_APPOINTMENTS_LOADED,
  UNCONFIRMED_APPOINTMENTS_LOADED,
  REFERRALS_IN_REVIEW_LOADED,
  REFERRALS_NOT_IN_REVIEW_LOADED,
  SHOW_ERROR,
  HAS_LOADED,
} from './mutation-types';


export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    try {
      const { confirmedAppointments, unconfirmedAppointments,
        referralsInReview, referralsNotInReview }
        = await this.app.$http.getV1PatientSecondaryCareSummary({ ignoreError: true });

      commit(REFERRALS_IN_REVIEW_LOADED, referralsInReview);
      commit(REFERRALS_NOT_IN_REVIEW_LOADED, referralsNotInReview);
      commit(CONFIRMED_APPOINTMENTS_LOADED, confirmedAppointments);
      commit(UNCONFIRMED_APPOINTMENTS_LOADED, unconfirmedAppointments);
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
