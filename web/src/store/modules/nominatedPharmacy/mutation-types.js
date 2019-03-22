export const NOMINATED_PHARMACY_LOADED = 'NOMINATED_PHARMACY_LOADED';
export const NOMINATED_PHARMACY_CLEAR = 'NOMINATED_PHARMACY_CLEAR';
export const SET_SEARCH_QUERY = 'SET_SEARCH_QUERY';
export const SET_SEARCH_RESULTS = 'SET_SEARCH_RESULTS';
export const initialState = () => ({
  pharmacy: {},
  hasLoaded: false,
  searchQuery: null,
  searchResults: {
    noResultsFound: false,
    pharmacies: [],
  },
});
