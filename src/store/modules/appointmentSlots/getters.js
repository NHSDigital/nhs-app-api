import { assign, find, map } from 'lodash/fp';

const findById = (id, collection) => find(item => item.id === id)(collection);
const findByIds = (ids, collection) => map(id => findById(id, collection))(ids);

export default {
  slots(state) {
    return state.slots.map((slot) => {
      const result = assign({}, slot);

      result.location = findById(slot.locationId, state.locations);
      result.appointmentSession = findById(
        slot.appointmentSessionId,
        state.appointmentSessions,
      );
      result.clinicians = findByIds(slot.clinicianIds, state.clinicians);

      return result;
    });
  },
  isSelected(state) {
    return id => id === state.selectedSlotId;
  },
  currentSlot(state) {
    return state.slots.find(slot => slot.id === state.selectedSlotId);
  },
};
