import { NOMINATED_PHARMACY_CLEAR, NOMINATED_PHARMACY_LOADED } from './mutation-types';

export default {
  load({ commit }) {
    return this.app.$http
      .getV1PharmacyNominated()
      .then((data) => {
        commit(NOMINATED_PHARMACY_LOADED, data);
      });
  },
  clear({ commit }) {
    commit(NOMINATED_PHARMACY_CLEAR);
  },
};
