export const CDSS_ADMIN = 'cdssAdmin';
export const CDSS_ADVICE = 'cdssAdvice';
export const IM1_PROVIDER = 'im1';
export const INFORMATICA = 'informatica';
export const INIT = 'INIT';
export const NOMINATED_PHARMACY = 'nominatedPharmacy';
export const SET_RULES = 'SET_RULES';

export const initialState = () => ({
  isLoaded: false,
  rules: {
    appointments: {
      provider: IM1_PROVIDER,
    },
    cdssAdvice: {
      provider: 'none',
    },
    cdssAdmin: {
      provider: 'none',
    },
    nominatedPharmacy: false,
  },
});
