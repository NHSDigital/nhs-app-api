import { LOADED, MAKE_DECISION } from './mutation-types';

export default {
  [LOADED](state, registration) {
    state.registration = registration;
  },
  [MAKE_DECISION](state, decision) {
    state.registration.decision = decision;
  },
};
