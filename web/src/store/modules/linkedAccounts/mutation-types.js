export const LOADED = 'LOADED';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const CLEAR_SELECTED_LINKED_ACCOUNT = 'CLEAR_SELECTED_LINKED_ACCOUNT';
export const SELECT = 'SELECT';
export const CLEAR_LINKED_ACCOUNTS = 'CLEAR_LINKED_ACCOUNTS';
export const initialState = () => ({
  items: [],
  selectedLinkedAccount: null,
  hasLoaded: false,
  hasErrored: false,
});
