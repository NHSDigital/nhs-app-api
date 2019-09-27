export const LOADED = 'LOADED';
export const INIT = 'INIT';

export const initialState = () => ({
  readMessages: [],
  unreadMessages: [],
  hasLoaded: false,
});
