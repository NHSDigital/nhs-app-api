export const IM1_PROVIDER = 'im1';
export const INIT = 'INIT';
export const ONLINE_CONSULTATIONS = 'online-consultations';
export const SET_RULES = 'SET_RULES';

export const initialState = () => ({
  isLoaded: false,
  rules: {
    appointments: {
      provider: IM1_PROVIDER,
    },
    onlineConsultations: true,
  },
});
