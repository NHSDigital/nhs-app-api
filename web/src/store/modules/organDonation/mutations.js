import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  SET_PRIVACY_ACCEPTANCE,
  SET_REGISTRATION_ID,
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
  [SET_ADDITIONAL_DETAILS](state, { ethnicityId, religionId }) {
    state.additionalDetails.ethnicityId = ethnicityId;
    state.additionalDetails.religionId = religionId;
  },
  [SET_ACCURACY_ACCEPTANCE](state, value) {
    state.isAccuracyAccepted = value;
  },
  [SET_ALL_ORGANS](state, choice) {
    state.registration.decisionDetails =
      { ...state.registration.decisionDetails, ...{ all: choice } };
  },
  [SET_PRIVACY_ACCEPTANCE](state, value) {
    state.isPrivacyAccepted = value;
  },
  [SET_REGISTRATION_ID](state, registrationId) {
    state.registration.identifier = registrationId;
  },
};
