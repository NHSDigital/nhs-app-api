export const INIT = 'INIT';
export const ACTIONABLE_REFERRALS_AND_APPOINTMENTS_LOADED = 'ACTIONABLE_REFERRALS_AND_APPOINTMENTS_LOADED';
export const CONFIRMED_APPOINTMENTS_LOADED = 'CONFIRMED_APPOINTMENTS_LOADED';
export const REFERRALS_IN_REVIEW_NOT_OVERDUE_LOADED = 'REFERRALS_IN_REVIEW_NOT_OVERDUE_LOADED';
export const SHOW_ERROR = 'SHOW_ERROR';
export const HAS_LOADED = 'HAS_LOADED';

export const initialState = () => ({
  summary: {
    actionableReferralsAndAppointments: [],
    confirmedAppointments: [],
    referralsInReviewNotOverdue: [],
  },
  apiError: null,
  hasLoaded: false,
});
