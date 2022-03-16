import {
  REFERRALS_LOADED,
  UPCOMING_APPOINTMENTS_LOADED,
  PAST_APPOINTMENTS_LOADED,
} from './mutation-types';


export default {
  async load({ commit }) {
    // insert real call here
    commit(REFERRALS_LOADED, []);
    commit(UPCOMING_APPOINTMENTS_LOADED, []);
    commit(PAST_APPOINTMENTS_LOADED, []);
  },
};
