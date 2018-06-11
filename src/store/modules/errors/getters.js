export default {
  showApiError(state) {
    if (state.apiErrors.length === 0) {
      return false;
    }

    return !state.hasConnectionProblem && state.showingApiErrorCondition(state.apiErrors[0].status);
  },
};
