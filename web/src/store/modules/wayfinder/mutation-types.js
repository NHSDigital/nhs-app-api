export const INIT = 'INIT';
export const REFERRALS_LOADED = 'REFERRALS_LOADED';
export const UPCOMING_APPOINTMENTS_LOADED = 'UPCOMING_APPOINTMENTS_LOADED';
export const SHOW_ERROR = 'SHOW_ERROR';
export const HAS_LOADED = 'HAS_LOADED';

export const initialState = () => ({
  summary: {
    referrals: [],
    upcomingAppointments: [],
  },
  apiError: undefined,
  hasLoaded: false,
});
