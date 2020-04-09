export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const END_VALIDATION_CHECKING = 'END_VALIDATION_CHECKING';
export const HIDE_EXPIRY_MESSAGE = 'HIDE_EXPIRY_MESSAGE';
export const SET_INFO = 'SET_INFO';
export const SET_LAST_CALLED_AT = 'SET_LAST_CALLED_AT';
export const SHOW_EXPIRY_MESSAGE = 'SHOW_EXPIRY_MESSAGE';
export const START_VALIDATION_CHECKING = 'START_VALIDATION_CHECKING';
export const SHOW_SESSION_EXPIRING = 'SHOW_SESSION_EXPIRING';
export const HIDE_SESSION_EXPIRING = 'HIDE_SESSION_EXPIRING';
export const P9_PROOF_LEVEL = 'P9';
export const initialState = () => ({
  hasLoaded: false,
  durationSeconds: undefined,
  gpOdsCode: undefined,
  lastCalledAt: undefined,
  validationInterval: undefined,
  proofLevel: undefined,
});
