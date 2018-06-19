import { CLEAR, LOADED, INIT } from './mutation-types';

export default {
  load({ commit }) {
    this.dispatch('myAppointments/init');
    return this.app.$http
      .getV1PatientAppointments({ includePastAppointments: false })
      .then((data) => {
        commit(LOADED, data);
      });
  },
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
};
