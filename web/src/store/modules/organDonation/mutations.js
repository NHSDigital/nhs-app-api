import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADED](state, registration) {
    state.registration = registration;
  },
  [LOADED_REFERENCE_DATA](state, referenceData) {
    state.referenceData = referenceData;
  },
  [MAKE_DECISION](state, decision) {
    state.additionalDetails = initialState().additionalDetails;
    state.registration.decision = decision;
  },
  [SET_ALL_ORGANS](state, choice) {
    state.registration.decisionDetails =
      { ...state.registration.decisionDetails, ...{ all: choice } };
  },
  [SET_ADDITIONAL_DETAILS](state, { ethnicityId, religionId }) {
    state.additionalDetails.ethnicityId = ethnicityId;
    state.additionalDetails.religionId = religionId;
  },
};
