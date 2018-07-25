export default {
  showApiError(state) {
    if (!state.showApiError || !state.pageSettings.showApiError || state.apiErrors.length === 0) {
      return false;
    }

    const error = state.apiErrors[0];
    const errorsStatusCollection = [409, 403, 460, 400];

    const isServerErrorStatus = error.status >= 500;
    const isExpectedStatus = errorsStatusCollection.indexOf(error.status) !== -1;
    const ignorePageError = state.pageSettings.ignoredErrors.indexOf(error.status) !== -1;

    return !state.hasConnectionProblem
      && (isServerErrorStatus || (isExpectedStatus && !ignorePageError));
  },
};
