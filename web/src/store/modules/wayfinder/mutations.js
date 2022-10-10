import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  ACTIONABLE_REFERRALS_AND_APPOINTMENTS_LOADED,
  CONFIRMED_APPOINTMENTS_LOADED,
  REFERRALS_IN_REVIEW_NOT_OVERDUE_LOADED,
  SHOW_ERROR,
  CLEAR_API_ERROR,
  HAS_LOADED,
  WAIT_TIMES,
  HAS_WAIT_TIMES_LOADED,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [ACTIONABLE_REFERRALS_AND_APPOINTMENTS_LOADED](state, actionableReferralsAndAppointments) {
    state.summary.actionableReferralsAndAppointments = actionableReferralsAndAppointments;
  },
  [CONFIRMED_APPOINTMENTS_LOADED](state, confirmedAppointments) {
    state.summary.confirmedAppointments = confirmedAppointments;
  },
  [REFERRALS_IN_REVIEW_NOT_OVERDUE_LOADED](state, referralsInReviewNotOverdue) {
    state.summary.referralsInReviewNotOverdue = referralsInReviewNotOverdue;
  },
  [SHOW_ERROR](state, apiError) {
    state.apiError = apiError;
  },
  [CLEAR_API_ERROR](state) {
    state.apiError = null;
  },
  [HAS_LOADED](state, hasLoaded) {
    state.hasLoaded = hasLoaded;
  },
  [HAS_WAIT_TIMES_LOADED](state, hasWaitTimesLoaded) {
    state.hasWaitTimesLoaded = hasWaitTimesLoaded;
  },
  [WAIT_TIMES](state, waitTimes) {
    if (waitTimes && waitTimes.waitTimes) {
      state.waitTimes = waitTimes.waitTimes;
    }
  },
};

