import {
  ADD_ERROR,
  BOOKING_JOURNEY_COMPLETE,
  BOOKING_JOURNEY_START,
  CLEAR,
  CLEAR_ERROR,
  DESELECT,
  FILTER,
  INIT,
  LOAD,
  SELECT,
  SET_BOOKING_REASON_NECESSITY,
  SET_SELECTED_OPTIONS,
} from './mutation-types';

const createError = ({ response }) => ({
  status: response.status || '',
  serviceDeskReference: response.data.serviceDeskReference || '',
});

export default {
  async book({ commit }, slot) {
    const param = {
      appointmentBookRequest: slot,
      ignoreError: true,
    };

    try {
      await this.app.$http.postV1PatientAppointments(param);
      commit(BOOKING_JOURNEY_START);
      if (process.client) {
        this.dispatch('analytics/satelliteTrack', 'appointment_booked');
      }
    } catch (error) {
      commit(ADD_ERROR, createError(error));
    }
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  clearError({ commit }) {
    commit(CLEAR_ERROR);
  },
  completeBookingJourney({ commit }) {
    commit(BOOKING_JOURNEY_COMPLETE);
  },
  deselect({ commit }) {
    commit(DESELECT);
  },
  filter({ commit }, selectedOptions) {
    commit(FILTER, selectedOptions);
  },
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    try {
      const data = await this.app.$http.getV1PatientAppointmentSlots({
        ignoreError: true,
      });
      commit(LOAD, data);
    } catch (error) {
      commit(ADD_ERROR, createError(error));
    }
  },
  select({ commit }, slot) {
    commit(SELECT, slot);
  },
  setBookingReasonNecessity({ commit }, value) {
    commit(SET_BOOKING_REASON_NECESSITY, value);
  },
  setSelectedFilters({ commit }, selectedOptions) {
    if (process.client) {
      this.dispatch('analytics/trackUserProperty', { key: 'appointmentDateFilterDropdownValue', value: selectedOptions.date });
    }
    commit(SET_SELECTED_OPTIONS, selectedOptions);
  },
  startBookingJourney({ commit }) {
    commit(BOOKING_JOURNEY_START);
  },
};
