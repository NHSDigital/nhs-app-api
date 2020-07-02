export default {
  shouldShowLeavingModal(state) {
    return typeof state.shouldSkipDisplayingLeavingWarning === 'boolean'
    && !state.shouldSkipDisplayingLeavingWarning;
  },
};
