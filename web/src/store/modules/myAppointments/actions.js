/* eslint-disable no-unused-vars */
import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_APPOINTMENT,
  CLEAR_APPOINTMENTS,
  CANCELLING_JOURNEY_COMPLETE,
} from './mutation-types';

export default {
  load({ commit }) {
    this.dispatch('myAppointments/init');
    return this.app.$http
      .getV1PatientAppointments()
      .then((data) => {
        commit(LOADED, data);
        return true;
      })
      .catch(() => false)
      .finally(() => {
        this.dispatch('device/unlockNavBar');
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
      appointmentCancelRequest: data,
    };

    return this.app.$http
      .deleteV1PatientAppointments(param).then(() => {
        commit('CANCELLING_JOURNEY_START');
        if (process.client) {
          this.dispatch('analytics/satelliteTrack', 'appointment_cancelled');
        }
      });
  },
  completeCancellingJourney({ commit }) {
    commit(CANCELLING_JOURNEY_COMPLETE);
  },
};
