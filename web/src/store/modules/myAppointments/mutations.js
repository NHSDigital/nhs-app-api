import { assign, find, get, map, mapKeys, sortBy } from 'lodash/fp';
import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_APPOINTMENT,
  CLEAR_APPOINTMENTS,
  CANCEL_SUCCESS,
  initialState,
} from './mutation-types';

const findById = (id, collection) => find(item => item.id === id)(collection);
const findByIds = (ids, collection) => map(id => findById(id, collection))(ids);
const sortSlots = sortBy(slot => [
  slot.startTime,
  get('clinicians[0].displayName')(slot),
]);
const clearAppointments = (state) => {
  state.appointmentSessions = [];
  state.clinicians = [];
  state.locations = [];
  state.appointments = [];
  state.hasLoaded = false;
  state.hasErrored = false;
};
const clearSelectedAppointment = (state) => {
  state.selectedAppointment = null;
  state.cancellationReasons = [];
};
export default {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.appointments = map((appointment) => {
      const result = assign({}, appointment);
      const location = findById(appointment.locationId, state.locations);
      if (location) {
        result.location = location;
      }

      const appointmentSession =
        findById(appointment.appointmentSessionId, state.appointmentSessions);
      if (appointmentSession) {
        result.appointmentSession = appointmentSession;
      }

      const clinicians = findByIds(appointment.clinicianIds, state.clinicians);
      if (clinicians && clinicians.length > 0) {
        result.clinicians = clinicians;
      }
      return result;
    })(state.appointments);
    state.appointments = sortSlots(state.appointments);
    state.hasLoaded = true;
  },
  [INIT](state) {
    state = initialState;
  },
  [CLEAR](state) {
    clearAppointments(state);
    clearSelectedAppointment(state);
    state.justCancelledAnAppointment = false;
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
  [CANCEL_SUCCESS](state) {
    state.justCancelledAnAppointment = true;
  },
};
