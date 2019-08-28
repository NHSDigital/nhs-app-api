export const CDSS_ADMIN = 'cdssAdmin';
export const CDSS_ADVICE = 'cdssAdvice';
export const GP_AT_HAND = 'gpAtHand';
export const IM1_PROVIDER = 'im1';
export const INFORMATICA = 'informatica';
export const INIT = 'INIT';
export const NOMINATED_PHARMACY = 'nominatedPharmacy';
export const SET_RULES = 'SET_RULES';
export const ONLINE_CONSULTATIONS = 'onlineConsultations';

export const initialState = () => ({
  isLoaded: false,
  rules: {
    appointments: {
      provider: IM1_PROVIDER,
    },
    cdssAdmin: {
      provider: 'none',
    },
    cdssAdvice: {
      provider: 'none',
    },
    nominatedPharmacy: false,
    prescriptions: {
      provider: IM1_PROVIDER,
    },
  },
});
