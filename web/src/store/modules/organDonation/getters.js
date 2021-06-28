import { DECISION_OPT_IN, DECISION_UNKNOWN, STATE_CONFLICTED } from './mutation-types';

export default {
  isSomeOrgans(state) {
    return state.registration.decision === DECISION_OPT_IN &&
      state.registration.decisionDetails.all === false;
  },
  canWithdraw(state) {
    return state.originalRegistration.decision !== DECISION_UNKNOWN &&
      state.originalRegistration.state !== STATE_CONFLICTED;
  },
};
