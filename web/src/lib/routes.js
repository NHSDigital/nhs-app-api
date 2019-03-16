import find from 'lodash/fp/find';

const routes = {
  ACCOUNT: {
    name: 'account',
    path: '/account',
  },
  ACCOUNT_SIGNOUT: {
    name: 'account-signout',
    path: '/account/signout',
  },
  APPOINTMENTS: {
    name: 'appointments',
    path: '/appointments',
  },
  APPOINTMENT_BOOKING_GUIDANCE: {
    name: 'appointments-booking-guidance',
    path: '/appointments/booking-guidance',
  },
  APPOINTMENT_CANCELLING: {
    name: 'appointments-cancelling',
    path: '/appointments/cancelling',
  },
  APPOINTMENT_CANCEL_NOJS: {
    name: 'appointments-cancel-noJs',
    path: '/nojs/appointments/cancel',
  },
  APPOINTMENT_BOOKING: {
    name: 'appointments-booking',
    path: '/appointments/booking',
  },
  APPOINTMENT_BOOK_NOJS: {
    name: 'appointments-book-noJs',
    path: '/nojs/appointments/book',
  },
  APPOINTMENT_CONFIRMATIONS: {
    name: 'appointments-confirmation',
    path: '/appointments/confirmation',
  },
  AUTH_RETURN: {
    name: 'auth-return',
    path: '/auth-return',
    isAnonymous: true,
  },
  BEGINLOGIN: {
    name: 'begin-login',
    path: '/begin-login',
    isAnonymous: true,
  },
  CHECKYOURSYMPTOMS: {
    name: 'check-your-symptoms',
    path: '/check-your-symptoms',
    isAnonymous: true,
  },
  DATA_SHARING_PREFERENCES: {
    name: 'data-sharing',
    path: '/data-sharing',
  },
  GP_FINDER: {
    name: 'gp-finder',
    path: '/gp-finder',
    isAnonymous: true,
  },
  GP_FINDER_RESULTS: {
    name: 'gp-finder-results',
    path: '/gp-finder/results',
    isAnonymous: true,
  },
  GP_FINDER_SENDING_EMAIL: {
    name: 'gp-finder-sending-email',
    path: '/gp-finder/sending-email',
    isAnonymous: true,
  },
  GP_FINDER_WAITING_LIST_JOINED: {
    name: 'gp-finder-waiting-list-joined',
    path: '/gp-finder/waiting-list-joined',
    isAnonymous: true,
  },
  GP_FINDER_PARTICIPATION: {
    name: 'gp-finder-participation',
    path: '/gp-finder/participation',
    isAnonymous: true,
  },
  INDEX: {
    name: 'index',
    path: '/',
  },
  LOGIN: {
    name: 'Login',
    path: '/login',
    isAnonymous: true,
  },
  LOGOUT: {
    name: 'logout',
    path: '/logout',
  },
  MORE: {
    name: 'more',
    path: '/more',
  },
  MYRECORD: {
    name: 'my-record',
    path: '/my-record',
  },
  MYRECORDNOACCESS: {
    name: 'my-record-noaccess',
    path: '/my-record/noaccess',
  },
  MYRECORDTESTRESULT: {
    name: 'my-record-testresultdetail',
    path: '/my-record/testresultdetail/:testResultId',
  },
  MY_RECORD_VISION_DIAGNOSIS_DETAIL: {
    name: 'my-record-diagnosis-detail',
    path: '/my-record/diagnosis-detail',
  },
  MY_RECORD_VISION_EXAMINATIONS_DETAIL: {
    name: 'my-record-examinations-detail',
    path: '/my-record/examinations-detail',
  },
  MY_RECORD_VISION_PROCEDURES_DETAIL: {
    name: 'my-record-procedures-detail',
    path: '/my-record/procedures-detail',
  },
  MY_RECORD_VISION_TEST_RESULTS_DETAIL: {
    name: 'my-record-test-results-detail',
    path: '/my-record/test-results-detail',
  },
  ORGAN_DONATION: {
    name: 'organ-donation',
    path: '/organ-donation',
  },
  ORGAN_DONATION_ADDITIONAL_DETAILS: {
    name: 'organ-donation-additional-details',
    path: '/organ-donation/additional-details',
  },
  ORGAN_DONATION_AMEND: {
    name: 'organ-donation-amend',
    path: '/organ-donation/amend',
  },
  ORGAN_DONATION_FAITH: {
    name: 'organ-donation-faith',
    path: '/organ-donation/faith',
  },
  ORGAN_DONATION_SOME_ORGANS: {
    name: 'organ-donation-some-organs',
    path: '/organ-donation/some-organs',
  },
  ORGAN_DONATION_REVIEW_YOUR_DECISION: {
    name: 'organ-donation-review-your-decision',
    path: '/organ-donation/review-your-decision',
  },
  ORGAN_DONATION_VIEW_DECISION: {
    name: 'organ-donation-view-decision',
    path: '/organ-donation/view-decision',
  },
  ORGAN_DONATION_WITHDRAW_REASON: {
    name: 'organ-donation-withdraw-reason',
    path: '/organ-donation/withdraw-reason',
  },
  ORGAN_DONATION_WITHDRAWN: {
    name: 'organ-donation-withdrawn',
    path: '/organ-donation/withdrawn',
  },
  ORGAN_DONATION_YOUR_CHOICE: {
    name: 'organ-donation-your-choice',
    path: '/organ-donation/your-choice',
  },
  PRESCRIPTIONS: {
    name: 'prescriptions',
    path: '/prescriptions',
  },
  PRESCRIPTION_REPEAT_COURSES: {
    name: 'prescriptions-repeat-courses',
    path: '/prescriptions/repeat-courses',
  },
  PRESCRIPTION_CONFIRM_COURSES: {
    name: 'prescriptions-confirm-prescription-details',
    path: '/prescriptions/confirm-prescription-details',
  },
  SYMPTOMS: {
    name: 'symptoms',
    path: '/symptoms',
  },
  TERMSANDCONDITIONS: {
    name: 'terms-and-conditions',
    path: '/terms-and-conditions',
  },

  // Legacy
  LEGACY_MYRECORDWARNING: {
    name: 'my-record-warning',
    path: '/my-record-warning',
  },
};

const findByName = name => find(({ name: routeName }) => routeName === name)(routes);

export const isAnonymous = (input) => {
  if (!input) return false;
  const route = input.name ? input : findByName(input);
  return route ? !!route.isAnonymous : false;
};

export const {
  ACCOUNT,
  ACCOUNT_SIGNOUT,
  APPOINTMENTS,
  APPOINTMENT_BOOK_NOJS,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CANCEL_NOJS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_CONFIRMATIONS,
  AUTH_RETURN,
  BEGINLOGIN,
  CHECKYOURSYMPTOMS,
  DATA_SHARING_PREFERENCES,
  GP_FINDER,
  GP_FINDER_RESULTS,
  GP_FINDER_PARTICIPATION,
  GP_FINDER_SENDING_EMAIL,
  GP_FINDER_WAITING_LIST_JOINED,
  INDEX,
  LEGACY_MYRECORDWARNING,
  LOGIN,
  LOGOUT,
  MORE,
  MYRECORD,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
  ORGAN_DONATION,
  ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_AMEND,
  ORGAN_DONATION_FAITH,
  ORGAN_DONATION_SOME_ORGANS,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_VIEW_DECISION,
  ORGAN_DONATION_WITHDRAW_REASON,
  ORGAN_DONATION_WITHDRAWN,
  ORGAN_DONATION_YOUR_CHOICE,
  PRESCRIPTIONS,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTION_CONFIRM_COURSES,
  SYMPTOMS,
  TERMSANDCONDITIONS,
} = routes;

