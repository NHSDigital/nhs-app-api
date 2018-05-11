import { assign, find, map } from 'lodash/fp';

const findById = (id, collection) => find(item => item.id === id)(collection);
const findByIds = (ids, collection) => map(id => findById(id, collection))(ids);

export const slots = state => state.slots.map((slot) => {
  const result = assign({}, slot);

  result.location = findById(slot.locationId, state.locations);
  result.appointmentSession = findById(slot.appointmentSessionId, state.appointmentSessions);
  result.clinicians = findByIds(slot.clinicianIds, state.clinicians);

  return result;
});

export const currentSlot = state => state.slots.find(slot => slot.id === state.selectedSlotId);

export const isSelected = state => id => id === state.selectedSlotId;

export default {
  isSelected,
  slots,
  currentSlot,
};
