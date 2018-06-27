import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_APPOINTMENT,
  CLEAR_APPOINTMENTS,
  CANCEL_SUCCESS,
} from './mutation-types';

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
  clearSelectedAppointment({ commit }) {
    commit(CLEAR_SELECTED_APPOINTMENT);
  },
  clearAppointments({ commit }) {
    commit(CLEAR_APPOINTMENTS);
  },
  cancel({ commit }, data) {
    /* eslint-disable no-unused-vars */
    const param = {
      AppointmentCancelRequest: data,
    };

    commit(CANCEL_SUCCESS);
    this.app.router.push('/appointments');
  },
};
