import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  CONFIRMED_APPOINTMENTS_LOADED,
  UNCONFIRMED_APPOINTMENTS_LOADED,
  REFERRALS_NOT_IN_REVIEW_LOADED,
  REFERRALS_IN_REVIEW_LOADED,
  SHOW_ERROR,
  HAS_LOADED,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [REFERRALS_IN_REVIEW_LOADED](state, referralsInReview) {
    state.summary.referralsInReview = referralsInReview;
  },
  [REFERRALS_NOT_IN_REVIEW_LOADED](state, referralsNotInReview) {
    state.summary.referralsNotInReview = referralsNotInReview;
  },
  [CONFIRMED_APPOINTMENTS_LOADED](state, confirmedAppointments) {
    state.summary.confirmedAppointments = confirmedAppointments;
  },
  [UNCONFIRMED_APPOINTMENTS_LOADED](state, unconfirmedAppointments) {
    state.summary.unconfirmedAppointments = unconfirmedAppointments;
  },
  [SHOW_ERROR](state, apiError) {
    state.apiError = apiError;
  },
  [HAS_LOADED](state, hasLoaded) {
    state.hasLoaded = hasLoaded;
  },
};

