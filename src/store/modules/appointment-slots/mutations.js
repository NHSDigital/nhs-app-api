import { assign, find, get, map, mapKeys, sortBy } from 'lodash/fp';
import { SLOT_SELECTED, SLOTS_LOADED } from './mutation-types';

const findById = (id, collection) => find(item => item.id === id)(collection);
const findByIds = (ids, collection) => map(id => findById(id, collection))(ids);
const sortSlots = sortBy(slot => [slot.startTime, get('clinicians[0].displayName')(slot)]);

export default {
  [SLOT_SELECTED](state, slotId) {
    state.selectedSlotId = slotId;
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
  },
};
