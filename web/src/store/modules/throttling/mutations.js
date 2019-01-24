import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  SET_WAITING_LIST_CHOICE,
  SET_SELECTED_GP_PRACTICE,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_SEARCH_QUERY](state, searchQuery) {
    state.searchQuery = searchQuery;
  },
  [SET_SEARCH_RESULTS](state, searchResults) {
    state.searchResults = searchResults;
  },
  [SET_WAITING_LIST_CHOICE](state, waitingListChoice) {
    state.waitingListChoice = waitingListChoice;
  },
  [SET_SELECTED_GP_PRACTICE](state, selectedGpPractice) {
    state.selectedGpPractice = selectedGpPractice;
  },
};
