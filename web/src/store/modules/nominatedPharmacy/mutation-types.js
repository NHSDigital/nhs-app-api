export const NOMINATED_PHARMACY_LOADED = 'NOMINATED_PHARMACY_LOADED';
export const NOMINATED_PHARMACY_CLEAR = 'NOMINATED_PHARMACY_CLEAR';
export const SET_SEARCH_QUERY = 'SET_SEARCH_QUERY';
export const SET_SEARCH_RESULTS = 'SET_SEARCH_RESULTS';
export const NOMINATED_PHARMACY_UPDATED = 'NOMINATED_PHARMACY_UPDATED';
export const CLEAR_SELECTED_NOMINATED_PHARMACY = 'CLEAR_SELECTED_NOMINATED_PHARMACY';
export const SELECT = 'SELECT';
export const SET_CHOSEN_TYPE = 'SET_CHOSEN_TYPE';
export const CLEAR_CHOSEN_TYPE = 'CLEAR_CHOSEN_TYPE';
export const HIGH_STREET_PHARMACY = 'highStreet';
export const ONLINE_PHARMACY = 'online';
export const SET_ONLINE_ONLY_KNOWN_OPTION = 'SET_ONLINE_ONLY_KNOWN_OPTION';

export const initialState = () => ({
  pharmacy: {},
  hasLoaded: false,
  selectedNominatedPharmacy: null,
  searchQuery: null,
  searchResults: {
    noResultsFound: false,
    pharmacies: [],
  },
  nominatedPharmacyEnabled: null,
  justUpdated: false,
  chosenType: undefined,
  onlineOnlyKnownOption: null,
});
