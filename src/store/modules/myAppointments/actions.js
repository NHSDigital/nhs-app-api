import { CLEAR, LOADED, INIT, SELECT, CLEAR_SELECTED_APPOINTMENT, CLEAR_APPOINTMENTS } from './mutation-types';

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
  select({ commit }, appointment) {
    commit(SELECT, appointment);
  },
  clearSelectedSlot({ commit }) {
    commit(CLEAR_SELECTED_APPOINTMENT);
  },
  clearAppointments({ commit }) {
    commit(CLEAR_APPOINTMENTS);
  },
};
