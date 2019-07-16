import find from 'lodash/fp/find';

const adminHelpRedirect = {
  condition: 'serviceJourneyRules/cdssAdminDisabled',
  url: '/appointments',
};

const gpAdviceRedirect = {
  condition: 'serviceJourneyRules/cdssAdviceDisabled',
  url: '/appointments',
};

const gpAtHandMyRecordRedirect = {
  condition: 'serviceJourneyRules/gpAtHandMyRecordEnabled',
  url: '/my-record/gp-at-hand',
};

const gpAtHandPrescriptionsRedirect = {
  condition: 'serviceJourneyRules/gpAtHandPrescriptionsEnabled',
  url: '/prescriptions/gp-at-hand',
};

const informaticaAppointmentRedirect = {
  condition: 'serviceJourneyRules/informaticaAppointmentsEnabled',
  url: '/appointments/informatica',
};

const im1AppointmentRedirect = {
  condition: 'serviceJourneyRules/im1AppointmentsEnabled',
  url: '/appointments',
};

const im1MyRecordRedirect = {
  condition: 'serviceJourneyRules/im1MyRecordEnabled',
  url: '/my-record',
};

const im1PrescriptionsRedirect = {
  condition: 'serviceJourneyRules/im1PrescriptionsEnabled',
  url: '/prescriptions',
};

const routes = {
  ACCOUNT: {
    name: 'account',
    path: '/account',
    crumb: {
      i8nKey: 'account',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
  },
  ACCOUNT_SIGNOUT: {
    name: 'account-signout',
    path: '/account/signout',
    crumb: {
      i8nKey: 'accountSignOut',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
  },
  APPOINTMENTS: {
    name: 'appointments',
    path: '/appointments',
    crumb: {
      i8nKey: 'appointments',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [
      informaticaAppointmentRedirect,
    ],
  },
  APPOINTMENT_ADMIN_HELP: {
    name: 'appointments-admin-help',
    path: '/appointments/admin-help',
    crumb: {
      i8nKey: 'appointmentsAdminHelp',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    redirectRules: [
      adminHelpRedirect,
    ],
  },
  APPOINTMENT_GP_ADVICE: {
    name: 'appointments-gp-advice',
    path: '/appointments/gp-advice',
    crumb: {
      i8nKey: 'appointmentsGpAdvice',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    redirectRules: [
      gpAdviceRedirect,
    ],
  },
  APPOINTMENT_BOOK_NOJS: {
    name: 'appointments-book-noJs',
    path: '/nojs/appointments/book',
    crumb: {
      i8nKey: 'appointmentsBooking',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
  },
  APPOINTMENT_BOOKING: {
    name: 'appointments-booking',
    path: '/appointments/booking',
    crumb: {
      i8nKey: 'appointmentsBooking',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
  },
  APPOINTMENT_BOOKING_GUIDANCE: {
    name: 'appointments-booking-guidance',
    path: '/appointments/booking-guidance',
    crumb: {
      enabled: true,
      i8nKey: 'appointmentsGuidanceBooking',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    redirectRules: [
      informaticaAppointmentRedirect,
    ],
  },
  APPOINTMENT_CANCEL_NOJS: {
    name: 'appointments-cancel-noJs',
    path: '/nojs/appointments/cancel',
    crumb: {
      i8nKey: 'appointmentsCancelling',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
  },
  APPOINTMENT_CANCELLING: {
    name: 'appointments-cancelling',
    path: '/appointments/cancelling',
    crumb: {
      i8nKey: 'appointmentsCancelling',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
  },
  APPOINTMENT_CONFIRMATIONS: {
    name: 'appointments-confirmation',
    path: '/appointments/confirmation',
    crumb: {
      i8nKey: 'appointmentsConfirmation',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
  },
  APPOINTMENT_INFORMATICA: {
    name: 'appointments-informatica',
    path: '/appointments/informatica',
    crumb: {
      enabled: true,
      i8nKey: 'appointmentsInformatica',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [
      im1AppointmentRedirect,
    ],
  },
  AUTH_RETURN: {
    name: 'auth-return',
    path: '/auth-return',
    isAnonymous: true,
    crumb: {
      i8nKey: 'authReturn',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
  },
  BEGINLOGIN: {
    name: 'begin-login',
    path: '/begin-login',
    isAnonymous: true,
    crumb: {
      i8nKey: 'beginLogin',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
  },
  CHECKYOURSYMPTOMS: {
    name: 'check-your-symptoms',
    path: '/check-your-symptoms',
    isAnonymous: true,
    crumb: {
      i8nKey: 'checkYourSymptoms',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
  },
  DATA_SHARING_PREFERENCES: {
    name: 'data-sharing',
    path: '/data-sharing',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  GP_FINDER: {
    name: 'gp-finder',
    path: '/gp-finder',
    isAnonymous: true,
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  GP_FINDER_RESULTS: {
    name: 'gp-finder-results',
    path: '/gp-finder/results',
    isAnonymous: true,
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  GP_FINDER_SENDING_EMAIL: {
    name: 'gp-finder-sending-email',
    path: '/gp-finder/sending-email',
    isAnonymous: true,
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  GP_FINDER_WAITING_LIST_JOINED: {
    name: 'gp-finder-waiting-list-joined',
    path: '/gp-finder/waiting-list-joined',
    isAnonymous: true,
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  GP_FINDER_PARTICIPATION: {
    name: 'gp-finder-participation',
    path: '/gp-finder/participation',
    isAnonymous: true,
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  INDEX: {
    name: 'index',
    path: '/',
    crumb: {
      i8nKey: 'home',
      get parentRoute() {
        return undefined;
      },
    },
  },
  LOGIN: {
    name: 'Login',
    path: '/login',
    isAnonymous: true,
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  LOGOUT: {
    name: 'logout',
    path: '/logout',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  MORE: {
    name: 'more',
    path: '/more',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  MYRECORD: {
    name: 'my-record',
    path: '/my-record',
    crumb: {
      i8nKey: 'myRecord',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [
      gpAtHandMyRecordRedirect,
    ],
  },
  MYRECORD_GP_AT_HAND: {
    name: 'my-record-gp-at-hand',
    path: '/my-record/gp-at-hand',
    crumb: {
      enabled: true,
      i8nKey: 'myRecordGpAtHand',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [
      im1MyRecordRedirect,
    ],
  },
  MYRECORDNOACCESS: {
    name: 'my-record-noaccess',
    path: '/my-record/noaccess',
    crumb: {
      i8nKey: 'myRecordNoAccess',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
  },
  MYRECORDTESTRESULT: {
    name: 'my-record-testresultdetail-testResultId',
    path: '/my-record/testresultdetail/:testResultId',
    crumb: {
      i8nKey: 'myRecordTestResult',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
  },
  MY_RECORD_VISION_DIAGNOSIS_DETAIL: {
    name: 'my-record-diagnosis-detail',
    path: '/my-record/diagnosis-detail',
    crumb: {
      i8nKey: 'myRecordDiagnosisDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
  },
  MY_RECORD_VISION_EXAMINATIONS_DETAIL: {
    name: 'my-record-examinations-detail',
    path: '/my-record/examinations-detail',
    crumb: {
      i8nKey: 'myRecordExaminationsDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
  },
  MY_RECORD_VISION_PROCEDURES_DETAIL: {
    name: 'my-record-procedures-detail',
    path: '/my-record/procedures-detail',
    crumb: {
      i8nKey: 'myRecordProceduresDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
  },
  MY_RECORD_VISION_TEST_RESULTS_DETAIL: {
    name: 'my-record-test-results-detail',
    path: '/my-record/test-results-detail',
    crumb: {
      i8nKey: 'myRecordTestResultsDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
  },
  NOMINATED_PHARMACY: {
    name: 'nominated-pharmacy',
    path: '/nominated-pharmacy',
    crumb: {
      i8nKey: 'nominatedPharmacy',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  NOMINATED_PHARMACY_SEARCH: {
    name: 'nominated-pharmacy-search',
    path: '/nominated-pharmacy/search',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  NOMINATED_PHARMACY_CONFIRM: {
    name: 'nominated-pharmacy-confirm',
    path: '/nominated-pharmacy/confirm',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  NOMINATED_PHARMACY_SEARCH_RESULTS: {
    name: 'nominated-pharmacy-results',
    path: '/nominated-pharmacy/results',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  NOMINATED_PHARMACY_CHECK: {
    name: 'nominated-pharmacy-check',
    path: '/nominated-pharmacy/check',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  NOMINATED_PHARMACY_CANNOT_CHANGE: {
    name: 'nominated-pharmacy-cannot-change',
    path: '/nominated-pharmacy/cannot-change',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  ORGAN_DONATION: {
    name: 'organ-donation',
    path: '/organ-donation',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_ADDITIONAL_DETAILS: {
    name: 'organ-donation-additional-details',
    path: '/organ-donation/additional-details',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_AMEND: {
    name: 'organ-donation-amend',
    path: '/organ-donation/amend',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_FAITH: {
    name: 'organ-donation-faith',
    path: '/organ-donation/faith',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_MORE_ABOUT_ORGANS: {
    name: 'organ-donation-more-about-organs',
    path: '/organ-donation/more-about-organs',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_SOME_ORGANS: {
    name: 'organ-donation-some-organs',
    path: '/organ-donation/some-organs',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_REVIEW_YOUR_DECISION: {
    name: 'organ-donation-review-your-decision',
    path: '/organ-donation/review-your-decision',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_VIEW_DECISION: {
    name: 'organ-donation-view-decision',
    path: '/organ-donation/view-decision',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_WITHDRAW_REASON: {
    name: 'organ-donation-withdraw-reason',
    path: '/organ-donation/withdraw-reason',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_WITHDRAWN: {
    name: 'organ-donation-withdrawn',
    path: '/organ-donation/withdrawn',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  ORGAN_DONATION_YOUR_CHOICE: {
    name: 'organ-donation-your-choice',
    path: '/organ-donation/your-choice',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
  },
  PRESCRIPTIONS: {
    name: 'prescriptions',
    path: '/prescriptions',
    crumb: {
      i8nKey: 'prescriptions',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [
      gpAtHandPrescriptionsRedirect,
    ],
  },
  PRESCRIPTION_CONFIRM_COURSES: {
    name: 'prescriptions-confirm-prescription-details',
    path: '/prescriptions/confirm-prescription-details',
    crumb: {
      i8nKey: 'prescriptionConfirmCourses',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  PRESCRIPTIONS_GP_AT_HAND: {
    name: 'prescriptions-gp-at-hand',
    path: '/prescriptions/gp-at-hand',
    crumb: {
      enabled: true,
      i8nKey: 'prescriptionsGpAtHand',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [
      im1PrescriptionsRedirect,
    ],
  },
  PRESCRIPTION_REPEAT_COURSES: {
    name: 'prescriptions-repeat-courses',
    path: '/prescriptions/repeat-courses',
    crumb: {
      i8nKey: 'prescriptionRepeatCourses',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
  },
  SYMPTOMS: {
    name: 'symptoms',
    path: '/symptoms',
    crumb: {
      i8nKey: 'symptoms',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
  },
  TERMSANDCONDITIONS: {
    name: 'terms-and-conditions',
    path: '/terms-and-conditions',
    crumb: {
      i8nKey: 'termsAndConditions',
      get parentRoute() {
        return undefined;
      },
    },
  },
  // Legacy
  LEGACY_MYRECORDWARNING: {
    name: 'my-record-warning',
    path: '/my-record-warning',
    crumb: {
      i8nKey: 'legacyMyRecordWarning',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    redirectRules: [
      gpAtHandMyRecordRedirect,
    ],
  },
};

/**
 * Wire up back reference to all routes for each route.
 */
Object.keys(routes).forEach((key) => { routes[key].crumb.allRoutes = routes; });

/**
 * Find route object by name.
 * @param name to find the representing object
 * @returns a single route object.
 */
export const findByName = name => find(({ name: routeName }) => routeName === name)(routes);

export const getRouteNames = () => Object.keys(routes).map(key => routes[key].name);

export const isAnonymous = (input) => {
  if (!input) return false;
  const route = input.name ? input : findByName(input);
  return !!(route || {}).isAnonymous;
};

/**
 * Used to define mappings for the home navigation
 * @param currentRouteName current route name
 * @returns {*|string} the resolved path for the home icon
 */
export const executeHomeNavigationRule = (currentRouteName) => {
  const mapping = {};
  mapping[routes.TERMSANDCONDITIONS.name] = routes.LOGOUT.path;
  return mapping[currentRouteName] || routes.INDEX.path;
};

/**
 * Derives the corresponding crumb trail for a given route.
 * Uses recursion to navigate the crumbtrail
 * @param route to calculate the corresponding crumb trail.
 * @returns {Array|*[]} crumbtrail model
 */
export const getCrumbTrailForRoute = (route) => {
  if (!(route && (route.crumb || {}).parentRoute)) {
    return [];
  }

  return getCrumbTrailForRoute(route.crumb.parentRoute).concat([route.crumb.parentRoute]);
};

export const {
  ACCOUNT,
  ACCOUNT_SIGNOUT,
  APPOINTMENTS,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_BOOK_NOJS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CANCEL_NOJS,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENT_GP_ADVICE,
  APPOINTMENT_INFORMATICA,
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
  MYRECORD_GP_AT_HAND,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
  NOMINATED_PHARMACY,
  NOMINATED_PHARMACY_SEARCH,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_SEARCH_RESULTS,
  NOMINATED_PHARMACY_CHECK,
  NOMINATED_PHARMACY_CANNOT_CHANGE,
  ORGAN_DONATION,
  ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_AMEND,
  ORGAN_DONATION_FAITH,
  ORGAN_DONATION_MORE_ABOUT_ORGANS,
  ORGAN_DONATION_SOME_ORGANS,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_VIEW_DECISION,
  ORGAN_DONATION_WITHDRAW_REASON,
  ORGAN_DONATION_WITHDRAWN,
  ORGAN_DONATION_YOUR_CHOICE,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTION_CONFIRM_COURSES,
  SYMPTOMS,
  TERMSANDCONDITIONS,
} = routes;
