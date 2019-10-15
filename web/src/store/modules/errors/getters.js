import isEmpty from 'lodash/fp/isEmpty';

const handledErrors = [464, 465];
const standardErrors = [400, 403, 409, 460, 461, 466];

export default {
  showApiError(state) {
    if (!state.showApiError) return false;
    if (!state.pageSettings.showApiError) return false;
    if (state.hasConnectionProblem) return false;
    if (isEmpty(state.apiErrors)) return false;

    const error = state.apiErrors[0];
    if (error.status >= 500) return true;

    const errorsStatusCollection = standardErrors.concat(handledErrors);

    const isExpectedStatus = errorsStatusCollection.indexOf(error.status) !== -1;
    const ignorePageError = state.pageSettings.ignoredErrors.indexOf(error.status) !== -1;

    return (isExpectedStatus && !ignorePageError);
  },
  isStandardError(state) {
    return state.apiErrors.length > 0 &&
     (standardErrors.includes(state.apiErrors[0].status) || state.apiErrors[0].status >= 500);
  },
};
