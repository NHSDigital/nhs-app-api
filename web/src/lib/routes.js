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
    name: 'appointments-cancel',
    path: '/appointments/cancel',
  },
  APPOINTMENT_BOOKING: {
    name: 'appointments-booking',
    path: '/appointments/booking',
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
  CHECKYOURSYMPTOMS: {
    name: 'check-your-symptoms',
    path: '/check-your-symptoms',
    isAnonymous: true,
  },
  DATA_SHARING_PREFERENCES: {
    name: 'data-sharing',
    path: '/data-sharing',
  },
  INDEX: {
    name: 'index',
    path: '/',
  },
  BEGINLOGIN: {
    name: 'begin-login',
    path: '/begin-login',
    isAnonymous: true,
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
  MYRECORDWARNING: {
    name: 'my-record-warning',
    path: '/my-record-warning',
  },
  MYRECORDNOACCESS: {
    name: 'my-record-noaccess',
    path: '/my-record/noaccess',
  },
  MYRECORDTESTRESULT: {
    name: 'my-record-testresultdetail',
    path: '/my-record/testresultdetail/:testResultId',
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
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_BOOKING,
  APPOINTMENT_CONFIRMATIONS,
  AUTH_RETURN,
  CHECKYOURSYMPTOMS,
  DATA_SHARING_PREFERENCES,
  INDEX,
  LOGIN,
  LOGOUT,
  BEGINLOGIN,
  MORE,
  MYRECORD,
  MYRECORDWARNING,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  PRESCRIPTIONS,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTION_CONFIRM_COURSES,
  SYMPTOMS,
  TERMSANDCONDITIONS,
} = routes;

