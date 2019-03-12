import cloneDeep from 'lodash/fp/cloneDeep';
import get from 'lodash/fp/get';
import isArray from 'lodash/fp/isArray';
import mapKeys from 'lodash/fp/mapKeys';
import set from 'lodash/fp/set';

import {
  CLONE_FROM_ORIGINAL,
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  RESET_REGISTRATION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  SET_AMENDING,
  SET_FAITH_DECLARATION,
  SET_PRIVACY_ACCEPTANCE,
  SET_REAFFIRMING,
  SET_REGISTRATION_ID,
  SET_SOME_ORGANS,
  SET_STATE,
  UPDATE_ORIGINAL_REGISTRATION,
  initialState,
} from './mutation-types';

export default {
  [CLONE_FROM_ORIGINAL](state, paths) {
    const lpaths = isArray(paths) ? paths : [paths];
    const updated = lpaths.reduce((aggregate, path) => {
      const original = get(path)(state.originalRegistration);
      if (original === undefined) {
        return aggregate;
      }
      const cloned = cloneDeep(original);
      return set(path)(cloned, aggregate);
    }, state.registration);

    state.registration = updated;
  },
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADED](state, registration) {
    state.registration = registration;
    state.originalRegistration = cloneDeep(state.registration);
  },
  [LOADED_REFERENCE_DATA](state, referenceData) {
    state.referenceData = referenceData;
  },
  [MAKE_DECISION](state, decision) {
    state.additionalDetails = initialState().additionalDetails;
    state.registration.decision = decision;
  },
  [RESET_REGISTRATION](state) {
    state.registration = initialState().registration;
  },
  [SET_ACCURACY_ACCEPTANCE](state, value) {
    state.isAccuracyAccepted = value;
  },
  [SET_ADDITIONAL_DETAILS](state, { ethnicityId, religionId }) {
    state.additionalDetails.ethnicityId = ethnicityId;
    state.additionalDetails.religionId = religionId;
  },
  [SET_ALL_ORGANS](state, choice) {
    state.registration.decisionDetails =
      { ...state.registration.decisionDetails,
        ...{ all: choice,
          choices: initialState().registration.decisionDetails.choices,
        },
      };
  },
  [SET_AMENDING](state, value) {
    state.isAmending = value;
  },
  [SET_FAITH_DECLARATION](state, faithDeclaration) {
    state.registration.faithDeclaration = faithDeclaration;
  },
  [SET_PRIVACY_ACCEPTANCE](state, value) {
    state.isPrivacyAccepted = value;
  },
  [SET_REAFFIRMING](state, value) {
    state.isReaffirming = value;
  },
  [SET_REGISTRATION_ID](state, registrationId) {
    state.registration.identifier = registrationId;
  },
  [SET_SOME_ORGANS](state, { value, choice }) {
    state.registration.decisionDetails.choices[choice] = value;
  },
  [SET_STATE](state, responseState) {
    state.registration.state = responseState;
  },
  [UPDATE_ORIGINAL_REGISTRATION](state) {
    state.originalRegistration = cloneDeep(state.registration);
  },
};
