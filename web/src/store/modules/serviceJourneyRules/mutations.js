import mapKeys from 'lodash/fp/mapKeys';
import { initialState,
  INIT,
  SET_RULES,
  SET_ADMIN_PROVIDER_NAME,
  SET_ADVICE_PROVIDER_NAME } from './mutation-types';

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

  [SET_ADMIN_PROVIDER_NAME](state, providerName) {
    state.rules.cdssAdmin.name = providerName;
  },
  [SET_ADVICE_PROVIDER_NAME](state, providerName) {
    state.rules.cdssAdvice.name = providerName;
  },
};
