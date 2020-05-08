export const UPDATE_REGISTRATION_STATUS = 'UPDATE_REGISTRATION_STATUS';
export const SET_WAITING = 'SET_WAITING';
export const UPDATE_BIOMETRIC_TYPE = 'UPDATE_BIOMETRIC_TYPE';
export const ADD_ERROR_CODE = 'ADD_ERROR_CODE';
export const CLEAR_ERROR_CODE = 'CLEAR_ERROR_CODE';
export const initialState = () => ({
  biometricsRegistrationStatus: false,
  biometricType: undefined,
  biometricLocaleReference: undefined,
  isWaiting: false,
  errorCode: undefined,
});
