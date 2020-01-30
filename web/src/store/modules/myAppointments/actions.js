import get from 'lodash/fp/get';
import {
  ADD_ERROR,
  CANCELLING_JOURNEY_COMPLETE,
  CANCELLING_JOURNEY_START,
  CLEAR,
  CLEAR_APPOINTMENTS,
  CLEAR_ERROR,
  CLEAR_SELECTED_APPOINTMENT,
  INIT,
  LOADED,
  SELECT,
} from './mutation-types';

const createError = ({ response }) => ({
  status: response.status || '',
  serviceDeskReference: get('serviceDeskReference')(response.data) || '',
});

export default {
  async cancel({ commit }, data) {
    const param = {
      appointmentCancelRequest: data,
      ignoreError: true,
    };

    try {
      await this.app.$http.deleteV1PatientAppointments(param);
      commit(CANCELLING_JOURNEY_START);
      if (process.client) {
        this.dispatch('analytics/satelliteTrack', 'appointment_cancelled');
      }
    } catch (error) {
      commit(ADD_ERROR, createError(error));
    }
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  clearAppointments({ commit }) {
    commit(CLEAR_APPOINTMENTS);
  },
  clearError({ commit }) {
    commit(CLEAR_ERROR);
  },
  clearSelectedAppointment({ commit }) {
    commit(CLEAR_SELECTED_APPOINTMENT);
  },
  completeCancellingJourney({ commit }) {
    commit(CANCELLING_JOURNEY_COMPLETE);
  },
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    this.dispatch('myAppointments/init');
    try {
      const data = await this.app.$http.getV1PatientAppointments({
        ignoreError: true,
      });
      commit(LOADED, data);
    } catch (error) {
      commit(ADD_ERROR, createError(error));
    } finally {
      this.dispatch('device/unlockNavBar');
    }
  },
  select({ commit }, appointment) {
    commit(SELECT, appointment);
  },
};
