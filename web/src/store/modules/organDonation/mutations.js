import { LOADED, LOADED_REFERENCE_DATA, MAKE_DECISION } from './mutation-types';

export default {
  [LOADED](state, registration) {
    state.registration = registration;
  },
  [LOADED_REFERENCE_DATA](state, referenceData) {
    state.referenceData = referenceData;
  },
  [MAKE_DECISION](state, decision) {
    state.registration.decision = decision;
  },
};
