export const INIT = 'INIT';
export const REFERRALS_NOT_IN_REVIEW_LOADED = 'REFERRALS_NOT_IN_REVIEW_LOADED';
export const REFERRALS_IN_REVIEW_LOADED = 'REFERRALS_IN_REVIEW_LOADED';
export const CONFIRMED_APPOINTMENTS_LOADED = 'CONFIRMED_APPOINTMENTS_LOADED';
export const UNCONFIRMED_APPOINTMENTS_LOADED = 'UNCONFIRMED_APPOINTMENTS_LOADED';
export const SHOW_ERROR = 'SHOW_ERROR';
export const HAS_LOADED = 'HAS_LOADED';

export const initialState = () => ({
  summary: {
    referralsNotInReview: [],
    referralsInReview: [],
    confirmedAppointments: [],
    unconfirmedAppointments: [],
  },
  apiError: null,
  hasLoaded: false,
});
