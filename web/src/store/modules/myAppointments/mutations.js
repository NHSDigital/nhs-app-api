import { get, mapKeys, sortBy } from 'lodash/fp';
import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_APPOINTMENT,
  CLEAR_APPOINTMENTS,
  initialState,
} from './mutation-types';

const sortSlots = sortBy(slot => [
  slot.startTime,
  get('clinicians[0].displayName')(slot),
]);
const clearAppointments = (state) => {
  state.pastAppointments = [];
  state.upcomingAppointments = [];
  state.hasLoaded = false;
  state.hasErrored = false;
  state.pastAppointmentsEnabled = false;
};
const clearSelectedAppointment = (state) => {
  state.selectedAppointment = null;
};
export default {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.upcomingAppointments = sortSlots(state.upcomingAppointments);
    state.pastAppointments = sortSlots(state.pastAppointments).reverse();
    state.hasLoaded = true;
  },
  [INIT](state) {
    state = initialState;
  },
  [CLEAR](state) {
    clearAppointments(state);
    clearSelectedAppointment(state);
  },
  [SELECT](state, selected) {
    state.selectedAppointment = selected;
  },
  [CLEAR_SELECTED_APPOINTMENT](state) {
    clearSelectedAppointment(state);
  },
  [CLEAR_APPOINTMENTS](state) {
    clearAppointments(state);
  },
};
