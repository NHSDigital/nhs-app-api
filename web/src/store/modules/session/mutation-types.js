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
export const SET_USER_SESSION_REFERENCE = 'SET_USER_SESSION_REFERENCE';
export const initialState = () => ({
  accessToken: undefined,
  csrfToken: undefined,
  dateOfBirth: undefined,
  durationSeconds: undefined,
  gpOdsCode: undefined,
  hasLoaded: false,
  lastCalledAt: undefined,
  nhsNumber: undefined,
  proofLevel: undefined,
  sessionTimeout: undefined,
  showExpiryMessage: false,
  showSessionExpiring: false,
  user: undefined,
  validationInterval: undefined,
  userSessionCreateReferenceCode: undefined,
});
