import sortBy from 'lodash/fp/sortBy';
import mapKeys from 'lodash/fp/mapKeys';
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
  state.pastAppointmentsEnabled = false;
  state.error = null;
};
const clearSelectedAppointment = (state) => {
  state.selectedAppointment = null;
};
export default {
  [ADD_ERROR](state, errorDetails) {
    state.error = errorDetails;
    state.hasLoaded = true;
  },
  [CANCELLING_JOURNEY_COMPLETE](state) {
    state.cancellingInProgress = false;
  },
  [CANCELLING_JOURNEY_START](state) {
    state.cancellingInProgress = true;
  },
  [CLEAR](state) {
    clearAppointments(state);
    clearSelectedAppointment(state);
  },
  [CLEAR_APPOINTMENTS](state) {
    clearAppointments(state);
  },
  [CLEAR_ERROR](state) {
    state.error = null;
  },
  [CLEAR_SELECTED_APPOINTMENT](state) {
    clearSelectedAppointment(state);
  },
  [INIT](state) {
    const blank = initialState();
    mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.upcomingAppointments = sortSlots(state.upcomingAppointments);
    state.pastAppointments = sortSlots(state.pastAppointments).reverse();
    state.hasLoaded = true;
  },
  [SELECT](state, selected) {
    state.selectedAppointment = selected;
  },
};
