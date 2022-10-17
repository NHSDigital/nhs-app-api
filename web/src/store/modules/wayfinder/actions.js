import { createLocalError } from '@/lib/utils';
import {
  INIT,
  ACTIONABLE_REFERRALS_AND_APPOINTMENTS_LOADED,
  CONFIRMED_APPOINTMENTS_LOADED,
  REFERRALS_IN_REVIEW_NOT_OVERDUE_LOADED,
  SHOW_ERROR,
  HAS_LOADED,
  WAIT_TIMES,
  CLEAR_API_ERROR,
  HAS_WAIT_TIMES_LOADED,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    try {
      const {
        actionableReferralsAndAppointments,
        confirmedAppointments,
        referralsInReviewNotOverdue,
      } = await this.app.$http.getV1PatientSecondaryCareSummary({ ignoreError: true });

      commit(ACTIONABLE_REFERRALS_AND_APPOINTMENTS_LOADED, actionableReferralsAndAppointments);
      commit(CONFIRMED_APPOINTMENTS_LOADED, confirmedAppointments);
      commit(REFERRALS_IN_REVIEW_NOT_OVERDUE_LOADED, referralsInReviewNotOverdue);
    } catch (error) {
      const apiError = createLocalError(error);

      if (apiError.status === 504 || apiError.status === 502 || apiError.status === 470) {
        commit(SHOW_ERROR, apiError);
      } else {
        this.dispatch('errors/addApiError', error);
      }
    } finally {
      commit(HAS_LOADED, true);
    }
  },
  async loadWaitTimes({ commit }) {
    try {
      const response = await this.app.$http.getV1PatientSecondaryCareWaittimes({ ignoreError: true });
      if (response) {
        commit(WAIT_TIMES, response);
      }
    } catch (error) {
      const apiError = createLocalError(error);

      if (apiError.status === 504 || apiError.status === 502 || apiError.status === 404) {
        commit(SHOW_ERROR, apiError);
      } else {
        this.dispatch('errors/addApiError', error);
      }
    } finally {
      commit(HAS_WAIT_TIMES_LOADED, true);
    }
  },
  clearApiError({ commit }, error) {
    commit(CLEAR_API_ERROR, error);
  },
};
