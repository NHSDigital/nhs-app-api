import isEmpty from 'lodash/fp/isEmpty';
import { standardErrors } from './mutation-types';

export default {
  showApiError(state) {
    if (!state.showApiError) return false;
    if (!state.pageSettings.showApiError) return false;
    if (state.hasConnectionProblem) return false;
    if (isEmpty(state.apiErrors)) return false;

    const { status } = state.apiErrors[0];

    return status >= 500 || state.pageSettings.ignoredErrors.indexOf(status) === -1;
  },
  isStandardError(state) {
    return state.apiErrors.length > 0 &&
     (standardErrors.includes(state.apiErrors[0].status) || state.apiErrors[0].status >= 500);
  },
};
