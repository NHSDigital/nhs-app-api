import {
  INIT,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  SET_WAITING_LIST_CHOICE,
  SET_SELECTED_GP_PRACTICE,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  setSearchQuery({ commit }, searchQuery) {
    commit(SET_SEARCH_QUERY, searchQuery);
  },
  setSearchResults({ commit }, searchResults) {
    commit(SET_SEARCH_RESULTS, searchResults);
  },
  setSelectedGpPractice({ commit }, selectedGpPractice) {
    commit(SET_SELECTED_GP_PRACTICE, selectedGpPractice);
  },
  setWaitingListChoice({ commit }, waitingListChoice) {
    commit(SET_WAITING_LIST_CHOICE, waitingListChoice);
  },
};
