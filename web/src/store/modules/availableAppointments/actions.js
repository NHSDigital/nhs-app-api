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
  BOOKING_JOURNEY_COMPLETE,
  BOOKING_JOURNEY_START,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  setSelectedFilters({ commit }, selectedOptions) {
    if (process.client) {
      this.dispatch('analytics/trackUserProperty', { key: 'appointmentDateFilterDropdownValue', value: selectedOptions.date });
    }
    commit(SET_SELECTED_OPTIONS, selectedOptions);
  },
  load({ commit }) {
    return this.app.$http
      .getV1PatientAppointmentSlots()
      .then((data) => {
        commit(LOAD, data);
      })
      .catch((error) => {
        this.dispatch('errors/addApiError', error);
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
        commit(BOOKING_JOURNEY_START);
        if (process.client) {
          this.dispatch('analytics/satelliteTrack', 'appointment_booked');
        }
      });
  },
  completeBookingJourney({ commit }) {
    commit(BOOKING_JOURNEY_COMPLETE);
  },
  startBookingJourney({ commit }) {
    commit(BOOKING_JOURNEY_START);
  },
};
