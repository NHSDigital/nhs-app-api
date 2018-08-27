const handledErrors = [464];
const standardErrors = [400, 403, 409, 460, 461];

export default {
  showApiError(state) {
    if (!state.showApiError || !state.pageSettings.showApiError || state.apiErrors.length === 0) {
      return false;
    }

    const error = state.apiErrors[0];
    const errorsStatusCollection = standardErrors.concat(handledErrors);

    const isServerErrorStatus = error.status >= 500;
    const isExpectedStatus = errorsStatusCollection.indexOf(error.status) !== -1;
    const ignorePageError = state.pageSettings.ignoredErrors.indexOf(error.status) !== -1;

    return !state.hasConnectionProblem
      && (isServerErrorStatus || (isExpectedStatus && !ignorePageError));
  },
  isStandardError(state) {
    return state.apiErrors.length > 0 &&
     (standardErrors.includes(state.apiErrors[0].status) || state.apiErrors[0].status >= 500);
  },
};
