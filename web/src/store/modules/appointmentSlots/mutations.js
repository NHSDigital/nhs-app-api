/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { assign, find, get, map, mapKeys, sortBy } from 'lodash/fp';
import { initialState, INIT_APPOINTMENTS, SLOTS_LOADED, SLOT_SELECTED, SLOTS_CLEAR } from './mutation-types';

const findById = (id, collection) => find(item => item.id === id)(collection);
const findByIds = (ids, collection) => map(id => findById(id, collection))(ids);
const sortSlots = sortBy(slot => [
  slot.startTime,
  get('clinicians[0].displayName')(slot),
]);

export default {
  [SLOT_SELECTED](state, slot) {
    state.selectedSlot = slot;
  },
  [SLOTS_CLEAR](state) {
    state.slots = [];
    state.hasLoaded = false;
    state.selectedSlot = null;
  },
  [INIT_APPOINTMENTS](state) {
    state = initialState;
  },
  [SLOTS_LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.slots = map((slot) => {
      const result = assign({}, slot);
      const location = findById(slot.locationId, state.locations);
      if (location) {
        result.location = location;
      }

      const appointmentSession = findById(slot.appointmentSessionId, state.appointmentSessions);
      if (appointmentSession) {
        result.appointmentSession = appointmentSession;
      }

      const clinicians = findByIds(slot.clinicianIds, state.clinicians);
      if (clinicians && clinicians.length > 0) {
        result.clinicians = clinicians;
      }

      return result;
    })(state.slots);
    state.slots = sortSlots(state.slots);
    state.hasLoaded = true;
    state.selectedSlot = null;
  },
};
