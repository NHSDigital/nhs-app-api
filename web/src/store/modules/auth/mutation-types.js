export const AUTH_RESPONSE = 'AUTH_RESPONSE';
export const INIT_AUTH = 'INIT_AUTH';
export const LOGOUT = 'LOGOUT';
export const SET_REDIRECT_URI = 'SET_REDIRECT_URI';
export const UPDATE_CONFIG = 'UPDATE_VERIFIER';

export const initialState = () => ({
  config: {},
  user: {},
  redirectUri: undefined,
  random: Math.random(),
});
