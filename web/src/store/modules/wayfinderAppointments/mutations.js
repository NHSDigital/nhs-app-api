import {
  REFERRALS_LOADED,
  UPCOMING_APPOINTMENTS_LOADED,
} from './mutation-types';


export default {
  [REFERRALS_LOADED](state, referrals) {
    state.referrals = referrals;
  },
  [UPCOMING_APPOINTMENTS_LOADED](state, referrals) {
    state.upcomingAppointments = referrals;
  },
};

