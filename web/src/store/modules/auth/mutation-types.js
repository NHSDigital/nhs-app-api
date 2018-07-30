export const initialState = {
  loggedIn: false,
  config: {},
  user: {},
  redirectUri: undefined,
};
export const AUTH_RESPONSE = 'AUTH_RESPONSE';
export const LOGOUT = 'LOGOUT';
export const SET_REDIRECT_URI = 'SET_REDIRECT_URI';
export const UPDATE_CONFIG = 'UPDATE_VERIFIER';
export const INIT_AUTH = 'INIT_AUTH';
