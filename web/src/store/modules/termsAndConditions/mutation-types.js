export const SET_ACCEPTANCE = 'SET_ACCEPTANCE';
export const SET_UPDATED_CONSENT_REQUIRED = 'SET_UPDATED_CONSENT_REQUIRED';
export const INIT_ACCEPTANCE = 'INIT_ACCEPTANCE';
export const initialState = () => ({
  areAccepted: false,
  analyticsCookieAccepted: '',
  updatedConsentRequired: false,
});
