export default {
  showApiError(state) {
    if (state.apiErrors.length === 0 || !state.showApiError) {
      return false;
    }

    const error = state.apiErrors[0];
    return !state.hasConnectionProblem && (error.status >= 500 || error.status === 403);
  },
};
