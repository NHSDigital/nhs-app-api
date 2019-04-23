export const NOMINATED_PHARMACY_LOADED = 'NOMINATED_PHARMACY_LOADED';
export const NOMINATED_PHARMACY_CLEAR = 'NOMINATED_PHARMACY_CLEAR';
export const SET_SEARCH_QUERY = 'SET_SEARCH_QUERY';
export const SET_SEARCH_RESULTS = 'SET_SEARCH_RESULTS';
export const NOMINATED_PHARMACY_UPDATED = 'NOMINATED_PHARMACY_UPDATED';
export const CLEAR_SELECTED_NOMINATED_PHARMACY = 'CLEAR_SELECTED_NOMINATED_PHARMACY';
export const SELECT = 'SELECT';
export const SET_PREVIOUS_PAGE_TO_SEARCH = 'SET_PREVIOUS_PAGE_TO_SEARCH';

export const initialState = () => ({
  pharmacy: {},
  hasLoaded: false,
  selectedNominatedPharmacy: null,
  searchQuery: null,
  searchResults: {
    noResultsFound: false,
    pharmacies: [],
  },
  previousPageToSearch: null,
});
