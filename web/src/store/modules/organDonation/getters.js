import { DECISION_OPT_IN } from './mutation-types';

export default {
  isSomeOrgans(state) {
    return state.registration.decision === DECISION_OPT_IN &&
      state.registration.decisionDetails.all === false;
  },
};
