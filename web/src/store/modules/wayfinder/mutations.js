import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  REFERRALS_LOADED,
  UPCOMING_APPOINTMENTS_LOADED,
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
  [REFERRALS_LOADED](state, referrals) {
    state.summary.referrals = referrals;
  },
  [UPCOMING_APPOINTMENTS_LOADED](state, upcomingAppointments) {
    state.summary.upcomingAppointments = upcomingAppointments;
  },
  [SHOW_ERROR](state, apiError) {
    state.apiError = apiError;
  },
  [HAS_LOADED](state, hasLoaded) {
    state.hasLoaded = hasLoaded;
  },
};

