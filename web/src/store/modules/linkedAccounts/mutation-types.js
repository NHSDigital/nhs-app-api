export const LOADED = 'LOADED';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const CLEAR_SELECTED_LINKED_ACCOUNT = 'CLEAR_SELECTED_LINKED_ACCOUNT';
export const SELECT = 'SELECT';
export const CLEAR_LINKED_ACCOUNTS = 'CLEAR_LINKED_ACCOUNTS';
export const LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY = 'LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY';
export const SET_LINKED_ACCOUNTS_CONFIG = 'SET_LINKED_ACCOUNTS_CONFIG';
export const SWITCH_TO_LINKED_ACCOUNT = 'SWITCH_TO_LINKED_ACCOUNT';
export const SWITCH_TO_MAIN_USER_ACCOUNT = 'SWITCH_TO_MAIN_USER_ACCOUNT';
export const initialState = () => ({
  items: [],
  selectedLinkedAccount: null,
  actingAsUser: null,
  hasErrored: false,
  config: {
    hasLoaded: false,
    patientId: '',
    hasLinkedAccounts: false,
  },
});
