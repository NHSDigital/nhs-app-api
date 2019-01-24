export const INIT = 'INIT';
export const SET_SEARCH_QUERY = 'SET_SEARCH_QUERY';
export const SET_SEARCH_RESULTS = 'SET_SEARCH_RESULTS';
export const SET_WAITING_LIST_CHOICE = 'SET_WAITING_LIST_CHOICE';
export const SET_SELECTED_GP_PRACTICE = 'SET_SELECTED_GP_PRACTICE';
export const initialState = () => ({
  searchQuery: undefined,
  searchResults: {
    technicalError: false,
    noResultsFound: false,
    tooManyResults: false,
    organisations: undefined,
  },
  selectedGpPractice: undefined,
  waitingListChoice: undefined,
});
