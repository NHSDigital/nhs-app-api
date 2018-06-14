export default {
  slots(state) {
    return state.slots;
  },
  isSelected(state) {
    return id => (state.selectedSlot !== null && id === state.selectedSlot.id);
  },
  currentSlot(state) {
    return state.selectedSlot;
  },
};
