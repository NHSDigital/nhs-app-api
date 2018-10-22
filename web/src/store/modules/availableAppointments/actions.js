/* eslint-disable no-unused-vars */
import {
  INIT,
  SELECT,
  DESELECT,
  LOAD,
  CLEAR,
  FILTER,
  SET_BOOKING_REASON_NECESSITY,
  SET_SELECTED_OPTIONS,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  setSelectedFilters({ commit }, selectedOptions) {
    commit(SET_SELECTED_OPTIONS, selectedOptions);
  },
  load({ commit }) {
    return this.app.$http
      .getV1PatientAppointmentSlots()
      .then((data) => {
        commit(LOAD, data);
      });
  },
  filter({ commit }, selectedOptions) {
    commit(FILTER, selectedOptions);
  },
  select({ commit }, slot) {
    commit(SELECT, slot);
  },
  setBookingReasonNecessity({ commit }, value) {
    commit(SET_BOOKING_REASON_NECESSITY, value);
  },
  deselect({ commit }) {
    commit(DESELECT);
  },
  book({ commit }, slot) {
    const param = {
      appointmentBookRequest: slot,
    };

    return this.app.$http
      .postV1PatientAppointments(param).then(() => {
        if (process.client) {
          this.dispatch('analytics/satelliteTrack', 'appointment_booked');
        }
      });
  },
};
