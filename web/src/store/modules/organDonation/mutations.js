import { LOADED, LOADED_REFERENCE_DATA, MAKE_DECISION, SET_ALL_ORGANS } from './mutation-types';

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
  [SET_ALL_ORGANS](state, choice) {
    state.registration.decisionDetails =
      { ...state.registration.decisionDetails, ...{ all: choice } };
  },
};
