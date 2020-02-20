import get from 'lodash/fp/get';
import find from 'lodash/fp/find';

const adminHelpDisabledRedirect = {
  journey_disabled: 'cdssAdmin',
  url: '/appointments',
};

const gpAdviceDisabledRedirect = {
  journey_disabled: 'cdssAdvice',
  url: '/appointments',
};

const gpAtHandAppointmentRedirect = {
  journey: 'gpAtHandAppointments',
  url: '/appointments/gp-at-hand',
};

const gpAtHandMyRecordRedirect = {
  journey: 'gpAtHandMyRecord',
  url: '/my-record/gp-at-hand',
};

const gpAtHandMedicalRecordRedirectV2 = {
  journey: 'gpAtHandGpMedicalRecordV2',
  url: '/gp-medical-record/gp-at-hand',
};

const gpMedicalRecordRedirectV2 = {
  journey: 'gpMedicalRecordV2',
  url: '/gp-medical-record',
};

const im1GpMedicalRecordRedirectV2 = {
  journey: 'im1GpMedicalRecordV2',
  url: '/gp-medical-record',
};

const gpAtHandPrescriptionsRedirect = {
  journey: 'gpAtHandPrescriptions',
  url: '/prescriptions/gp-at-hand',
};

const im1AppointmentRedirect = {
  journey: 'im1Appointments',
  url: '/appointments',
};

const im1MyRecordRedirect = {
  journey: 'im1MyRecord',
  url: '/my-record',
};

const im1PrescriptionsRedirect = {
  journey: 'im1Prescriptions',
  url: '/prescriptions',
};

const informaticaAppointmentRedirect = {
  journey: 'informaticaAppointments',
  url: '/appointments/informatica',
};

const messagingDisabledRedirect = {
  journey_disabled: 'messaging',
  url: '/',
};

const linkedAccountAppointmentRedirect = {
  journey: 'linkedAccountAppointments',
  url: '/linked-profiles/shutter/appointments',
};

const baseNhsAppHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

const routes = {
  ACCOUNT: {
    name: 'account',
    path: '/account',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'account',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      url: '/linked-profiles/shutter/settings',
    }],
    helpUrl: `${baseNhsAppHelpUrl}account/`,
  },
  ACCOUNT_COOKIES: {
    name: 'account-cookies',
    path: '/account/cookies',
    crumb: {
      nativeDisabled: false,
      i18nKey: 'accountCookies',
      get parentRoute() {
        return this.allRoutes.ACCOUNT;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}account/`,
  },
  ACCOUNT_NOTIFICATIONS: {
    name: 'account-notifications',
    path: '/account/notifications',
    crumb: {
      i18nKey: 'accountNotifications',
      get parentRoute() {
        return this.allRoutes.ACCOUNT;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}account/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }, {
      condition: 'session/isProxying',
      value: true,
      url: '/linked-profiles/shutter/settings',
    }],
    sjrRedirectRules: [{
      journey_disabled: 'notifications',
      url: '/',
    }],
  },
  ACCOUNT_SIGNOUT: {
    name: 'account-signout',
    path: '/account/signout',
    crumb: {
      i18nKey: 'accountSignOut',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}account/`,
  },
  ACUTE_MEDICINES: {
    name: 'gp-medical-record-medicines-acute-medicines',
    path: '/gp-medical-record/medicines/acute-medicines',
    crumb: {
      i18nKey: 'medicines',
      get parentRoute() {
        return this.allRoutes.MEDICINES;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  APPOINTMENTS: {
    name: 'appointments',
    path: '/appointments',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointments',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      linkedAccountAppointmentRedirect,
      gpAtHandAppointmentRedirect,
      informaticaAppointmentRedirect,
    ],
    proxyShutterPath: '/linked-profiles/shutter/appointments',
  },
  APPOINTMENT_ADMIN_HELP: {
    name: 'appointments-admin-help',
    path: '/appointments/admin-help',
    warningBanner: true,
    crumb: {
      i18nKey: 'appointmentsAdminHelp',
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}online-consultations/`,
    sjrRedirectRules: [
      adminHelpDisabledRedirect,
    ],
  },
  APPOINTMENT_GP_ADVICE: {
    name: 'appointments-gp-advice',
    path: '/appointments/gp-advice',
    warningBanner: true,
    crumb: {
      i18nKey: 'appointmentsGpAdvice',
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}online-consultations/`,
    sjrRedirectRules: [
      gpAdviceDisabledRedirect,
    ],
  },
  APPOINTMENT_BOOKING: {
    name: 'appointments-booking',
    path: '/appointments/booking',
    crumb: {
      i18nKey: 'appointmentsBooking',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_BOOKING_GUIDANCE: {
    name: 'appointments-booking-guidance',
    path: '/appointments/booking-guidance',
    crumb: {
      enabled: true,
      i18nKey: 'appointmentsGuidanceBooking',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      gpAtHandAppointmentRedirect,
      informaticaAppointmentRedirect,
    ],
  },
  APPOINTMENT_CANCEL_NOJS: {
    name: 'appointments-cancel-noJs',
    path: '/nojs/appointments/cancel',
    crumb: {
      i18nKey: 'appointmentsCancelling',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_CANCELLING: {
    name: 'appointments-cancelling',
    path: '/appointments/cancelling',
    crumb: {
      i18nKey: 'appointmentsCancelling',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_CANCELLING_SUCCESS: {
    name: 'appointments-cancelling-success',
    path: '/appointments/cancelling-success',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointmentsConfirmation',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    redirectRules: [{
      condition: 'myAppointments/isCancellingAppointmentInProgress',
      value: false,
      url: '/appointments',
    }],
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_CONFIRMATIONS: {
    name: 'appointments-confirmation',
    path: '/appointments/confirmation',
    crumb: {
      i18nKey: 'appointmentsConfirmation',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_BOOKING_SUCCESS: {
    name: 'appointments-booking-success',
    path: '/appointments/booking-success',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointmentsConfirmation',
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    redirectRules: [{
      condition: 'availableAppointments/isBookingAppointmentInProgress',
      value: false,
      url: '/appointments',
    }],
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_GP_AT_HAND: {
    name: 'appointments-gp-at-hand',
    path: '/appointments/gp-at-hand',
    crumb: {
      enabled: true,
      i18nKey: 'appointmentsGpAtHand',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      im1AppointmentRedirect,
      informaticaAppointmentRedirect,
    ],
  },
  APPOINTMENT_INFORMATICA: {
    name: 'appointments-informatica',
    path: '/appointments/informatica',
    crumb: {
      enabled: true,
      i18nKey: 'appointmentsInformatica',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      gpAtHandAppointmentRedirect,
      im1AppointmentRedirect,
    ],
  },
  AUTH_RETURN: {
    name: 'auth-return',
    path: '/auth-return',
    isAnonymous: true,
    crumb: {
      i18nKey: 'authReturn',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  BEGINLOGIN: {
    name: 'begin-login',
    path: '/begin-login',
    isAnonymous: true,
    crumb: {
      i18nKey: 'beginLogin',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  CURRENT_MEDICINES: {
    name: 'gp-medical-record-medicines-current-medicines',
    path: '/gp-medical-record/medicines/current-medicines',
    crumb: {
      i18nKey: 'current_medicines',
      get parentRoute() {
        return this.allRoutes.MEDICINES;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  CHECKYOURSYMPTOMS: {
    name: 'check-your-symptoms',
    path: '/check-your-symptoms',
    isAnonymous: true,
    crumb: {
      i18nKey: 'checkYourSymptoms',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  CONSULTATIONS: {
    name: 'gp-medical-record-consultations',
    path: '/gp-medical-record/consultations',
    crumb: {
      i18nKey: 'consultations',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DATA_SHARING_PREFERENCES: {
    name: 'data-sharing',
    path: '/data-sharing',
    crumb: {
      get parentRoute() {
        return this.allRoutes.MORE;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}ndop/`,
  },
  EVENTS: {
    name: 'gp-medical-record-events',
    path: '/gp-medical-record/events',
    crumb: {
      i18nKey: 'events',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DISCONTINUED_MEDICINES: {
    name: 'gp-medical-record-medicines-discontinued-medicines',
    path: '/gp-medical-record/medicines/discontinued-medicines',
    crumb: {
      i18nKey: 'medicines',
      get parentRoute() {
        return this.allRoutes.MEDICINES;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  INDEX: {
    name: 'index',
    path: '/',
    crumb: {
      i18nKey: 'home',
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  INTERSTITIAL_REDIRECTOR: {
    name: 'redirector',
    path: '/redirector',
    crumb: {
      nativeDisabled: true,
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  LOGIN: {
    name: 'Login',
    path: '/login',
    isAnonymous: true,
    shouldShowContentHeader: false,
    crumb: {
      nativeDisabled: true,
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  LOGOUT: {
    name: 'logout',
    path: '/logout',
    crumb: {
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  MESSAGING: {
    name: 'messaging',
    path: '/messaging',
    crumb: {
      i18nKey: 'messaging',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    sjrRedirectRules: [
      messagingDisabledRedirect,
    ],
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
    helpUrl: baseNhsAppHelpUrl,
  },
  MESSAGING_MESSAGES: {
    name: 'messaging-messages',
    path: '/messaging/messages',
    shouldShowContentHeader: false,
    crumb: {
      get parentRoute() {
        return this.allRoutes.MESSAGING;
      },
    },
    sjrRedirectRules: [
      messagingDisabledRedirect,
    ],
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
    helpUrl: baseNhsAppHelpUrl,
  },
  MORE: {
    name: 'more',
    path: '/more',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'more',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      url: '/linked-profiles/shutter/more',
    }],
  },
  MYRECORD: {
    name: 'my-record',
    path: '/my-record',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'myRecord',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [
      gpAtHandMedicalRecordRedirectV2,
      gpAtHandMyRecordRedirect,
      gpMedicalRecordRedirectV2,
    ],
  },
  GP_MEDICAL_RECORD: {
    name: 'gp-medical-record',
    path: '/gp-medical-record',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'myRecord',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MYRECORD_GP_AT_HAND: {
    name: 'my-record-gp-at-hand',
    path: '/my-record/gp-at-hand',
    crumb: {
      enabled: true,
      i18nKey: 'myRecordGpAtHand',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [
      im1MyRecordRedirect,
    ],
  },
  GP_MEDICAL_RECORD_GP_AT_HAND: {
    name: 'gp-medical-record-gp-at-hand',
    path: '/gp-medical-record/gp-at-hand',
    crumb: {
      enabled: true,
      i18nKey: 'myRecordGpAtHand',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [
      im1GpMedicalRecordRedirectV2,
    ],
  },
  MYRECORDNOACCESS: {
    name: 'my-record-noaccess',
    path: '/my-record/noaccess',
    crumb: {
      i18nKey: 'myRecordNoAccess',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  ALLERGIESANDREACTIONS: {
    name: 'gp-medical-record-allergies-and-reactions',
    path: '/gp-medical-record/allergies-and-reactions',
    crumb: {
      i18nKey: 'allergiesAndReactions',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  ENCOUNTERS: {
    name: 'gp-medical-record-encounters',
    path: '/gp-medical-record/encounters',
    crumb: {
      i18nKey: 'encounters',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  TESTRESULTS: {
    name: 'gp-medical-record-test-results',
    path: '/gp-medical-record/test-results',
    crumb: {
      i18nKey: 'testResults',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  TESTRESULTSDETAIL: {
    name: 'gp-medical-record-test-results-detail',
    path: '/gp-medical-record/test-results-detail',
    crumb: {
      i18nKey: 'gpMedicalRecordTestResultsDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  TESTRESULTID: {
    name: 'gp-medical-record-testresultdetail-testResultId',
    path: '/gp-medical-record/testresultdetail/:testResultId',
    crumb: {
      i18nKey: 'gpMedicalRecordTestResult',
      get parentRoute() {
        return this.allRoutes.TESTRESULTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  IMMUNISATIONS: {
    name: 'gp-medical-record-immunisations',
    path: '/gp-medical-record/immunisations',
    crumb: {
      i18nKey: 'immunisations',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DIAGNOSIS_V2: {
    name: 'gp-medical-record-diagnosis',
    path: '/gp-medical-record/diagnosis',
    crumb: {
      i18nKey: 'diagnosis',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  EXAMINATIONS_V2: {
    name: 'gp-medical-record-examinations',
    path: '/gp-medical-record/examinations',
    crumb: {
      i18nKey: 'examinations',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  PROCEDURES_V2: {
    name: 'gp-medical-record-procedures',
    path: '/gp-medical-record/procedures',
    crumb: {
      i18nKey: 'procedures',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MEDICINES: {
    name: 'gp-medical-record-medicines',
    path: '/gp-medical-record/medicines-index',
    crumb: {
      i18nKey: 'medicines',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MEDICAL_HISTORY: {
    name: 'gp-medical-record-medical-history',
    path: '/gp-medical-record/medical-history',
    crumb: {
      i18nKey: 'medicalHistory',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MYRECORDTESTRESULT: {
    name: 'my-record-testresultdetail-testResultId',
    path: '/my-record/testresultdetail/:testResultId',
    crumb: {
      i18nKey: 'myRecordTestResult',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_DIAGNOSIS_DETAIL: {
    name: 'my-record-diagnosis-detail',
    path: '/my-record/diagnosis-detail',
    crumb: {
      i18nKey: 'myRecordDiagnosisDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_EXAMINATIONS_DETAIL: {
    name: 'my-record-examinations-detail',
    path: '/my-record/examinations-detail',
    crumb: {
      i18nKey: 'myRecordExaminationsDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_PROCEDURES_DETAIL: {
    name: 'my-record-procedures-detail',
    path: '/my-record/procedures-detail',
    crumb: {
      i18nKey: 'myRecordProceduresDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_TEST_RESULTS_DETAIL: {
    name: 'my-record-test-results-detail',
    path: '/my-record/test-results-detail',
    crumb: {
      i18nKey: 'myRecordTestResultsDetail',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DOCUMENTS: {
    name: 'gp-medical-record-documents',
    path: '/gp-medical-record/documents',
    crumb: {
      i18nKey: 'myRecordDocuments',
      get parentRoute() {
        return this.allRoutes.GP_MEDICAL_RECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DOCUMENT: {
    name: 'gp-medical-record-documents-id',
    path: '/gp-medical-record/documents/:id',
    crumb: {
      get parentRoute() {
        return this.allRoutes.DOCUMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DOCUMENT_DETAIL: {
    name: 'gp-medical-record-documents-detail-id',
    path: '/gp-medical-record/documents/detail/:id',
    shouldShowContentHeader: false,
    crumb: {
      get parentRoute() {
        return this.allRoutes.DOCUMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  NOMINATED_PHARMACY: {
    name: 'nominated-pharmacy',
    path: '/nominated-pharmacy',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_SEARCH: {
    name: 'nominated-pharmacy-search',
    path: '/nominated-pharmacy/search',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CONFIRM: {
    name: 'nominated-pharmacy-confirm',
    path: '/nominated-pharmacy/confirm',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CHANGE_SUCCESS: {
    name: 'nominated-pharmacy-change-success',
    path: '/nominated-pharmacy/change-success',
    crumb: {},
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_INTERRUPT: {
    name: 'nominated-pharmacy-interrupt',
    path: '/nominated-pharmacy/interrupt',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_DSP_INTERRUPT: {
    name: 'nominated-pharmacy-dsp-interrupt',
    path: '/nominated-pharmacy/dsp-interrupt',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_SEARCH_RESULTS: {
    name: 'nominated-pharmacy-results',
    path: '/nominated-pharmacy/results',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CHECK: {
    name: 'nominated-pharmacy-check',
    path: '/nominated-pharmacy/check',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CHOOSE_TYPE: {
    name: 'nominated-pharmacy-choose-type',
    path: '/nominated-pharmacy/choose-type',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES: {
    name: 'nominated-pharmacy-online-only-choices',
    path: '/nominated-pharmacy/online-only-choices',
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH: {
    name: 'nominated-pharmacy-online-only-search',
    path: '/nominated-pharmacy/online-only-search',
    shouldShowContentHeader: false,
    crumb: {
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  ORGAN_DONATION: {
    name: 'organ-donation',
    path: '/organ-donation',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.MORE;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_ADDITIONAL_DETAILS: {
    name: 'organ-donation-additional-details',
    path: '/organ-donation/additional-details',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_AMEND: {
    name: 'organ-donation-amend',
    path: '/organ-donation/amend',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_FAITH: {
    name: 'organ-donation-faith',
    path: '/organ-donation/faith',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_MORE_ABOUT_ORGANS: {
    name: 'organ-donation-more-about-organs',
    path: '/organ-donation/more-about-organs',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_SOME_ORGANS: {
    name: 'organ-donation-some-organs',
    path: '/organ-donation/some-organs',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_REVIEW_YOUR_DECISION: {
    name: 'organ-donation-review-your-decision',
    path: '/organ-donation/review-your-decision',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_VIEW_DECISION: {
    name: 'organ-donation-view-decision',
    path: '/organ-donation/view-decision',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.MORE;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_WITHDRAW_REASON: {
    name: 'organ-donation-withdraw-reason',
    path: '/organ-donation/withdraw-reason',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_WITHDRAWN: {
    name: 'organ-donation-withdrawn',
    path: '/organ-donation/withdrawn',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_YOUR_CHOICE: {
    name: 'organ-donation-your-choice',
    path: '/organ-donation/your-choice',
    crumb: {
      i18nKey: 'organ_donation',
      get parentRoute() {
        return this.allRoutes.ORGAN_DONATION;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  PATIENT_PRACTICE_MESSAGING: {
    name: 'patient-practice-messaging',
    path: '/patient-practice-messaging',
    crumb: {
      i18nKey: 'patientPracticeMessaging',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_DELETE: {
    name: 'patient-practice-messaging-delete',
    path: '/patient-practice-messaging/delete',
    crumb: {
      i18nKey: 'patientPracticeMessagingDelete',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_URGENCY: {
    name: 'patient-practice-messaging-urgency',
    path: '/patient-practice-messaging/urgency',
    crumb: {
      i18nKey: 'patientPracticeMessagingUrgency',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_URGENCY_CONTACT_GP: {
    name: 'patient-practice-messaging-urgency-contact-your-gp',
    path: '/patient-practice-messaging/urgency/contact-your-gp',
    crumb: {
      i18nKey: 'patientPracticeMessagingUrgencyContactYourGp',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_RECIPIENTS: {
    name: 'patient-practice-messaging-recipients',
    path: '/patient-practice-messaging/recipients',
    crumb: {
      i18nKey: 'patientPracticeMessagingRecipients',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE: {
    name: 'patient-practice-messaging-view-details',
    path: '/patient-practice-messaging/view-details',
    crumb: {
      i18nKey: 'patientPracticeMessagingViewDetails',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_CREATE: {
    name: 'patient-practice-messaging-send-message',
    path: '/patient-practice-messaging/send-message',
    crumb: {
      i18nKey: 'patientPracticeMessagingCreate',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PATIENT_PRACTICE_MESSAGING_DELETE_SUCCESS: {
    name: 'patient-practice-messaging-delete-success',
    path: '/patient-practice-messaging/delete-success',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'patientPracticeMessagingDeleteSuccess',
      get parentRoute() {
        return this.allRoutes.PATIENT_PRACTICE_MESSAGING;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  PRESCRIPTIONS: {
    name: 'prescriptions',
    path: '/prescriptions',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'prescriptions',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    sjrRedirectRules: [
      gpAtHandPrescriptionsRedirect,
    ],
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
  },
  PRESCRIPTION_CONFIRM_COURSES: {
    name: 'prescriptions-confirm-prescription-details',
    path: '/prescriptions/confirm-prescription-details',
    crumb: {
      i18nKey: 'prescriptionConfirmCourses',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  PRESCRIPTIONS_GP_AT_HAND: {
    name: 'prescriptions-gp-at-hand',
    path: '/prescriptions/gp-at-hand',
    crumb: {
      enabled: true,
      i18nKey: 'prescriptionsGpAtHand',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    sjrRedirectRules: [
      im1PrescriptionsRedirect,
    ],
  },
  PRESCRIPTION_REPEAT_COURSES: {
    name: 'prescriptions-repeat-courses',
    path: '/prescriptions/repeat-courses',
    crumb: {
      i18nKey: 'prescriptionRepeatCourses',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS: {
    name: 'prescriptions-repeat-partial-success',
    path: '/prescriptions/repeat-partial-success',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'prescriptionRepeatPartialSuccess',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    redirectRules: [{
      condition: 'repeatPrescriptionCourses/isOrderPrescriptionInProgress',
      value: false,
      url: '/prescriptions',
    }],
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  PRESCRIPTIONS_ORDER_SUCCESS: {
    name: 'prescriptions-order-success',
    path: '/prescriptions/order-success',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'prescriptionsOrderSuccess',
      get parentRoute() {
        return this.allRoutes.PRESCRIPTIONS;
      },
    },
    redirectRules: [{
      condition: 'repeatPrescriptionCourses/isOrderPrescriptionInProgress',
      value: false,
      url: '/prescriptions',
    }],
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  RECALLS: {
    name: 'gp-medical-record-recalls',
    path: '/gp-medical-record/recalls',
    crumb: {
      i18nKey: 'recalls',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  REFERRALS: {
    name: 'gp-medical-record-referrals',
    path: '/gp-medical-record/referrals',
    crumb: {
      i18nKey: 'referrals',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  HEALTH_CONDITIONS: {
    name: 'gp-medical-record-health-conditions',
    path: '/gp-medical-record/health-conditions',
    crumb: {
      i18nKey: 'healthConditions',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  SYMPTOMS: {
    name: 'symptoms',
    path: '/symptoms',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'symptoms',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      url: '/linked-profiles/shutter/symptoms',
    }],
  },
  TERMSANDCONDITIONS: {
    name: 'terms-and-conditions',
    path: '/terms-and-conditions',
    crumb: {
      i18nKey: 'termsAndConditions',
      get parentRoute() {
        return undefined;
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  LINKED_PROFILES: {
    name: 'linked-profiles',
    path: '/linked-profiles',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.ACCOUNT;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SUMMARY: {
    name: 'linked-profiles-summary',
    path: '/linked-profiles/summary',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.LINKED_PROFILES;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_MORE: {
    name: 'linked-profiles-shutter-more',
    path: '/linked-profiles/shutter/more',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_SYMPTOMS: {
    name: 'linked-profiles-shutter-symptoms',
    path: '/linked-profiles/shutter/symptoms',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_SETTINGS: {
    name: 'linked-profiles-shutter-settings',
    path: '/linked-profiles/shutter/settings',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_APPOINTMENTS: {
    name: 'linked-profiles-shutter-appointments',
    path: '/linked-profiles/shutter/appointments',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS: {
    name: 'linked-profiles-shutter-prescriptions',
    path: '/linked-profiles/shutter/prescriptions',
    crumb: {
      i18nKey: 'linkedProfiles',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  SWITCH_PROFILE: {
    name: 'switch-profile',
    path: '/switch-profile',
    crumb: {
      i18nKey: 'switchProfile',
      get parentRoute() {
        return this.allRoutes.INDEX;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  // Legacy
  LEGACY_MYRECORDWARNING: {
    name: 'my-record-warning',
    path: '/my-record-warning',
    crumb: {
      i18nKey: 'legacyMyRecordWarning',
      get parentRoute() {
        return this.allRoutes.MYRECORD;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [
      gpAtHandMyRecordRedirect,
    ],
  },
};

/**
 * Overrides for the back link on native
 *
 * For these routes, when clicking the back nav on native, the defaultPath will be used if
 * there is no state.navigation.backLinkOverride path set in the store. If this value is
 * to be ignored and the default path is to always be used, be sure to set ignoreStore: true
 */
export const backLinkOverrides = {
  [routes.ORGAN_DONATION.name]: {
    defaultPath: routes.MORE.path,
  },
  [routes.ORGAN_DONATION_VIEW_DECISION.name]: {
    defaultPath: routes.MORE.path,
  },
  [routes.PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.name]: {
    ignoreStore: true,
    defaultPath: routes.PATIENT_PRACTICE_MESSAGING.path,
  },
  [routes.SWITCH_PROFILE.name]: {
    ignoreStore: true,
    defaultPath: routes.INDEX.path,
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

export const findByPath = path => find(({ path: pathValue }) => pathValue === path)(routes);

export const getRouteNames = () => Object.keys(routes).map(key => routes[key].name);

export const REDIRECT_PARAMETER = 'redirect_to';
export const getRedirectRoute = (route) => {
  const redirectPath = get(REDIRECT_PARAMETER)(route.query);
  return redirectPath ? findByName(redirectPath) || {} : {};
};

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
  ACCOUNT_COOKIES,
  ACCOUNT_NOTIFICATIONS,
  ACCOUNT_SIGNOUT,
  ALLERGIESANDREACTIONS,
  ACUTE_MEDICINES,
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CANCELLING_SUCCESS,
  APPOINTMENT_CANCEL_NOJS,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENT_BOOKING_SUCCESS,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_GP_ADVICE,
  APPOINTMENT_GP_AT_HAND,
  APPOINTMENT_INFORMATICA,
  AUTH_RETURN,
  BEGINLOGIN,
  CHECKYOURSYMPTOMS,
  CONSULTATIONS,
  EVENTS,
  CURRENT_MEDICINES,
  DATA_SHARING_PREFERENCES,
  DIAGNOSIS_V2,
  DISCONTINUED_MEDICINES,
  ENCOUNTERS,
  EXAMINATIONS_V2,
  INDEX,
  INTERSTITIAL_REDIRECTOR,
  LEGACY_MYRECORDWARNING,
  LOGIN,
  LOGOUT,
  MEDICINES,
  MEDICAL_HISTORY,
  MESSAGING,
  MESSAGING_MESSAGES,
  MORE,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
  DOCUMENTS,
  DOCUMENT,
  DOCUMENT_DETAIL,
  NOMINATED_PHARMACY,
  NOMINATED_PHARMACY_SEARCH,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES,
  NOMINATED_PHARMACY_CHANGE_SUCCESS,
  NOMINATED_PHARMACY_INTERRUPT,
  NOMINATED_PHARMACY_DSP_INTERRUPT,
  NOMINATED_PHARMACY_SEARCH_RESULTS,
  NOMINATED_PHARMACY_CHECK,
  NOMINATED_PHARMACY_CHOOSE_TYPE,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH,
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
  PATIENT_PRACTICE_MESSAGING,
  PATIENT_PRACTICE_MESSAGING_URGENCY,
  PATIENT_PRACTICE_MESSAGING_URGENCY_CONTACT_GP,
  PATIENT_PRACTICE_MESSAGING_RECIPIENTS,
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE,
  PATIENT_PRACTICE_MESSAGING_CREATE,
  PATIENT_PRACTICE_MESSAGING_DELETE,
  PATIENT_PRACTICE_MESSAGING_DELETE_SUCCESS,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS,
  PRESCRIPTIONS_ORDER_SUCCESS,
  PROCEDURES_V2,
  RECALLS,
  REFERRALS,
  HEALTH_CONDITIONS,
  SYMPTOMS,
  TERMSANDCONDITIONS,
  TESTRESULTS,
  TESTRESULTSDETAIL,
  TESTRESULTID,
  IMMUNISATIONS,
  LINKED_PROFILES,
  LINKED_PROFILES_SUMMARY,
  LINKED_PROFILES_SHUTTER_MORE,
  LINKED_PROFILES_SHUTTER_SYMPTOMS,
  LINKED_PROFILES_SHUTTER_SETTINGS,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS,
  SWITCH_PROFILE,
} = routes;
