import mapKeys from 'lodash/fp/mapKeys';
import { initialState,
  INIT,
  SET_RULES,
  SET_PATIENT_GUID } from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();

    mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_RULES](state, rules) {
    state.rules = rules.journeys;
    state.isLoaded = true;
  },
  [SET_PATIENT_GUID](state, id) {
    state.patientGuid = id;
  },
};
