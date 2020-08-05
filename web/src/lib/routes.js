import { find, get } from 'lodash/fp';
import consola from 'consola';
import proofLevel from './proofLevel';
import sjrIf from './sjrIf';
import { AppPage } from '../static/js/v1/src/constants';

const baseNhsAppHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

const sjrRedirectRules = {
  adminHelpDisabledRedirect: {
    journey_disabled: 'cdssAdmin',
    url: '/appointments',
  },
  deleteMessageRedirect: {
    journey_disabled: 'deleteGpMessages',
    url: '/messages/gp-messages',
  },
  documentsDisabledRedirect: {
    journey_disabled: 'documents',
    url: '/health-records/gp-medical-record',
  },
  gpAdviceDisabledRedirect: {
    journey_disabled: 'cdssAdvice',
    url: '/appointments/gp-appointments',
  },
  gpAtHandAppointmentRedirect: {
    journey: 'gpAtHandAppointments',
    url: '/appointments/gp-at-hand',
  },
  gpAtHandMyRecordRedirect: {
    journey: 'gpAtHandMyRecord',
    url: '/my-record/gp-at-hand',
  },
  gpAtHandMedicalRecordRedirectV2: {
    journey: 'gpAtHandGpMedicalRecordV2',
    url: '/health-records/gp-at-hand',
  },
  gpAtHandPrescriptionsRedirect: {
    journey: 'gpAtHandPrescriptions',
    url: '/prescriptions/gp-at-hand',
  },
  gpMedicalRecordRedirectV2: {
    journey: 'gpMedicalRecordV2',
    url: '/health-records/gp-medical-record',
  },
  im1AppointmentRedirect: {
    journey: 'im1Appointments',
    url: '/appointments/gp-appointments',
  },
  im1GpMedicalRecordRedirectV2: {
    journey: 'im1GpMedicalRecordV2',
    url: '/health-records/gp-medical-record',
  },
  im1MessagingDisabledRedirect: {
    journey_disabled: 'im1Messaging',
    url: '/',
  },
  im1MyRecordRedirect: {
    journey: 'im1MyRecord',
    url: '/my-record',
  },
  im1PrescriptionsRedirect: {
    journey: 'im1Prescriptions',
    url: '/prescriptions',
  },
  informaticaAppointmentRedirect: {
    journey: 'informaticaAppointments',
    url: '/appointments/informatica',
  },
  messagingDisabledRedirect: {
    journey_disabled: 'messaging',
    url: '/',
  },
  linkedAccountAppointmentRedirect: {
    journey: 'linkedAccountAppointments',
    url: '/linked-profiles/shutter/appointments',
  },
  silverIntegrationsSecondaryAppointmentsDisabledRedirect: {
    journey_disabled: 'silverIntegrationAppointments',
    url: '/appointments',
  },
  silverIntegrationsHealthRecordHubCarePlansEnabledRedirect: {
    journey: 'myRecordHub',
    url: '/health-records',
  },
};

const routes = {
  ACCOUNT: {
    name: 'account',
    path: '/account',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'account',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
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
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: false,
      i18nKey: 'accountCookies',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.ACCOUNT];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}account/`,
  },
  ACCOUNT_NOTIFICATIONS: {
    name: 'account-notifications',
    path: '/account/notifications',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'accountNotifications',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.ACCOUNT];
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
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'accountSignOut',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}account/`,
  },
  ACUTE_MEDICINES: {
    name: 'health-records-gp-medical-record-medicines-acute-medicines',
    path: '/health-records/gp-medical-record/medicines/acute-medicines',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'medicines',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  APPOINTMENTS: {
    name: 'appointments',
    path: '/appointments',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointments',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  GP_APPOINTMENTS: {
    name: 'appointments-gp-appointments',
    path: '/appointments/gp-appointments',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      nativeDisabled: false,
      i18nKey: 'gpAppointments',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS];
      },
    },
    proxyShutterPath: '/linked-profiles/shutter/appointments',
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      sjrRedirectRules.linkedAccountAppointmentRedirect,
      sjrRedirectRules.gpAtHandAppointmentRedirect,
      sjrRedirectRules.informaticaAppointmentRedirect,
    ],
  },
  HOSPITAL_APPOINTMENTS: {
    name: 'appointments-hospital-appointments',
    path: '/appointments/hospital-appointments',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      nativeDisabled: false,
      i18nKey: 'hospitalAppointments',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS];
      },
      get parentRoute() {
        return this.allRoutes.APPOINTMENTS;
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [sjrRedirectRules.silverIntegrationsSecondaryAppointmentsDisabledRedirect],
  },
  APPOINTMENT_ADMIN_HELP: {
    name: 'appointments-gp-appointments-admin-help',
    path: '/appointments/gp-appointments/admin-help',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    warningBanner: true,
    crumb: {
      i18nKey: 'appointmentsAdminHelp',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
      get moreCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}online-consultations/`,
    sjrRedirectRules: [sjrRedirectRules.adminHelpDisabledRedirect],
  },
  APPOINTMENT_GP_ADVICE: {
    name: 'appointments-gp-appointments-gp-advice',
    path: '/appointments/gp-appointments/gp-advice',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    warningBanner: true,
    crumb: {
      i18nKey: 'appointmentsGpAdvice',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
      get symptomsCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.SYMPTOMS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}online-consultations/`,
    sjrRedirectRules: [sjrRedirectRules.gpAdviceDisabledRedirect],
  },
  APPOINTMENT_BOOKING: {
    name: 'appointments-gp-appointments-booking',
    path: '/appointments/gp-appointments/booking',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsBooking',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_BOOKING_GUIDANCE: {
    name: 'appointments-gp-appointments-booking-guidance',
    path: '/appointments/gp-appointments/booking-guidance',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsGuidanceBooking',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      sjrRedirectRules.gpAtHandAppointmentRedirect,
      sjrRedirectRules.informaticaAppointmentRedirect,
    ],
  },
  APPOINTMENT_CANCEL_NOJS: {
    name: 'appointments-cancel-noJs',
    path: '/nojs/appointments/cancel',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsCancelling',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_CANCELLING: {
    name: 'appointments-gp-appointments-cancelling',
    path: '/appointments/gp-appointments/cancelling',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsCancelling',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_CANCELLING_SUCCESS: {
    name: 'appointments-gp-appointments-cancelling-success',
    path: '/appointments/gp-appointments/cancelling-success',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointmentsConfirmation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    redirectRules: [{
      condition: 'myAppointments/isCancellingAppointmentInProgress',
      value: false,
      url: '/appointments/gp-appointments',
    }],
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_CONFIRMATIONS: {
    name: 'appointments-gp-appointments-confirmation',
    path: '/appointments/gp-appointments/confirmation',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsConfirmation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_BOOKING_SUCCESS: {
    name: 'appointments-gp-appointments-booking-success',
    path: '/appointments/gp-appointments/booking-success',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointmentsConfirmation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.APPOINTMENTS, this.allRoutes.GP_APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    redirectRules: [{
      condition: 'availableAppointments/isBookingAppointmentInProgress',
      value: false,
      url: '/appointments/gp-appointments',
    }],
  },
  APPOINTMENT_ADD_TO_CALENDAR: {
    name: 'appointments-gp-appointments-add-to-calendar-interrupt',
    path: '/appointments/gp-appointments/add-to-calendar-interrupt',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      nativeDisabled: false,
      i18nKey: 'appointmentsConfirmation',
      get defaultCrumb() {
        return [this.allRoutes.APPOINTMENT_BOOKING_SUCCESS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  APPOINTMENT_GP_AT_HAND: {
    name: 'appointments-gp-at-hand',
    path: '/appointments/gp-at-hand',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsGpAtHand',
      get defaultCrumb() {
        return [this.allRoutes.APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      sjrRedirectRules.im1AppointmentRedirect,
      sjrRedirectRules.informaticaAppointmentRedirect,
    ],
  },
  APPOINTMENT_INFORMATICA: {
    name: 'appointments-informatica',
    path: '/appointments/informatica',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/appointments',
    crumb: {
      i18nKey: 'appointmentsInformatica',
      get defaultCrumb() {
        return [this.allRoutes.APPOINTMENTS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
    sjrRedirectRules: [
      sjrRedirectRules.gpAtHandAppointmentRedirect,
      sjrRedirectRules.im1AppointmentRedirect,
    ],
  },
  AUTH_RETURN: {
    name: 'auth-return',
    path: '/auth-return',
    isAnonymous: true,
    crumb: {
      i18nKey: 'authReturn',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
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
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  CURRENT_MEDICINES: {
    name: 'health-records-gp-medical-record-medicines-current-medicines',
    path: '/health-records/gp-medical-record/medicines/current-medicines',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'current_medicines',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
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
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  CONSULTATIONS: {
    name: 'health-records-gp-medical-record-consultations',
    path: '/health-records/gp-medical-record/consultations',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'consultations',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DATA_SHARING_OVERVIEW: {
    name: 'data-sharing',
    path: '/data-sharing',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'dataSharingOverview',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}ndop/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
  },
  DATA_SHARING_WHERE_USED: {
    name: 'data-sharing-where-used',
    path: '/data-sharing/where-used',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'dataSharingWhereUsed',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}ndop/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
  },
  DATA_SHARING_DOES_NOT_APPLY: {
    name: 'data-sharing-does-not-apply',
    path: '/data-sharing/does-not-apply',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'dataSharingDoesNotApply',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}ndop/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
  },
  DATA_SHARING_MAKE_YOUR_CHOICE: {
    name: 'data-sharing-make-your-choice',
    path: '/data-sharing/make-your-choice',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'dataSharingMakeYourChoice',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}ndop/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
  },
  EVENTS: {
    name: 'health-records-gp-medical-record-events',
    path: '/health-records/gp-medical-record/events',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'events',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DISCONTINUED_MEDICINES: {
    name: 'health-records-gp-medical-record-medicines-discontinued-medicines',
    path: '/health-records/gp-medical-record/medicines/discontinued-medicines',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'medicines',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  INDEX: {
    name: 'index',
    path: '/',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'home',
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  INTERSTITIAL_REDIRECTOR: {
    name: 'redirector',
    path: '/redirector',
    proofLevel: proofLevel.P5,
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}third-party/`,
  },
  LOGIN: {
    name: 'Login',
    path: '/login',
    isAnonymous: true,
    shouldShowContentHeader: false,
    crumb: {
      nativeDisabled: true,
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  LOGOUT: {
    name: 'logout',
    path: '/logout',
    proofLevel: proofLevel.P5,
    crumb: {
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  MESSAGES: {
    name: 'messages',
    path: '/messages',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'messages',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
  },
  HEALTH_INFORMATION_UPDATES: {
    name: 'messages-app-messaging',
    path: '/messages/app-messaging',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'healthAndInformationUpdates',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MESSAGES];
      },
      get appMessagesOnlyCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.messagingDisabledRedirect],
  },
  HEALTH_INFORMATION_UPDATES_MESSAGES: {
    name: 'messages-app-messaging-app-message',
    path: '/messages/app-messaging/app-message',
    proofLevel: proofLevel.P5,
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MESSAGES,
          this.allRoutes.HEALTH_INFORMATION_UPDATES];
      },
      get appMessagesOnlyCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.HEALTH_INFORMATION_UPDATES];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.messagingDisabledRedirect],
  },
  MORE: {
    name: 'more',
    path: '/more',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'more',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
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
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'myRecord',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [
      sjrRedirectRules.silverIntegrationsHealthRecordHubCarePlansEnabledRedirect,
      sjrRedirectRules.gpMedicalRecordRedirectV2,
      sjrRedirectRules.gpAtHandMedicalRecordRedirectV2,
      sjrRedirectRules.gpAtHandMyRecordRedirect,
    ],
  },
  GP_MEDICAL_RECORD: {
    name: 'health-records-gp-medical-record',
    path: '/health-records/gp-medical-record',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      nativeDisabled({ $store }) {
        const rule = sjrRedirectRules.silverIntegrationsHealthRecordHubCarePlansEnabledRedirect;
        return sjrIf({
          $store,
          journey: rule.journey,
          disabled: true,
        });
      },
      i18nKey: 'myRecord',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  HEALTH_RECORDS: {
    name: 'health-records',
    path: '/health-records',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'healthRecords',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}health-records/`,
  },
  MYRECORD_GP_AT_HAND: {
    name: 'my-record-gp-at-hand',
    path: '/my-record/gp-at-hand',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      nativeDisabled({ $store }) {
        const rule = sjrRedirectRules.silverIntegrationsHealthRecordHubCarePlansEnabledRedirect;
        return sjrIf({
          $store,
          journey: rule.journey,
          disabled: true,
        });
      },
      i18nKey: 'myRecordGpAtHand',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [sjrRedirectRules.im1MyRecordRedirect],
  },
  GP_MEDICAL_RECORD_GP_AT_HAND: {
    name: 'health-records-gp-at-hand',
    path: '/health-records/gp-at-hand',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordGpAtHand',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [sjrRedirectRules.im1GpMedicalRecordRedirectV2],
  },
  MYRECORDNOACCESS: {
    name: 'my-record-noaccess',
    path: '/my-record/noaccess',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordNoAccess',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  ALLERGIESANDREACTIONS: {
    name: 'health-records-gp-medical-record-allergies-and-reactions',
    path: '/health-records/gp-medical-record/allergies-and-reactions',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'allergiesAndReactions',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  ENCOUNTERS: {
    name: 'health-records-gp-medical-record-encounters',
    path: '/health-records/gp-medical-record/encounters',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'encounters',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  TESTRESULTS: {
    name: 'health-records-gp-medical-record-test-results',
    path: '/health-records/gp-medical-record/test-results',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'testResults',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  TESTRESULTSDETAIL: {
    name: 'health-records-gp-medical-record-test-results-detail',
    path: '/health-records/gp-medical-record/test-results-detail',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'gpMedicalRecordTestResultsDetail',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  TESTRESULTID: {
    name: 'health-records-gp-medical-record-testresultdetail-testResultId',
    path: '/health-records/gp-medical-record/testresultdetail/:testResultId',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'gpMedicalRecordTestResult',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  IMMUNISATIONS: {
    name: 'health-records-gp-medical-record-immunisations',
    path: '/health-records/gp-medical-record/immunisations',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'immunisations',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DIAGNOSIS_V2: {
    name: 'health-records-gp-medical-record-diagnosis',
    path: '/health-records/gp-medical-record/diagnosis',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'diagnosis',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  EXAMINATIONS_V2: {
    name: 'health-records-gp-medical-record-examinations',
    path: '/health-records/gp-medical-record/examinations',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'examinations',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  PROCEDURES_V2: {
    name: 'health-records-gp-medical-record-procedures',
    path: '/health-records/gp-medical-record/procedures',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'procedures',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MEDICINES: {
    name: 'health-records-gp-medical-record-medicines',
    path: '/health-records/gp-medical-record/medicines',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'medicines',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MEDICAL_HISTORY: {
    name: 'health-records-gp-medical-record-medical-history',
    path: '/health-records/gp-medical-record/medical-history',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'medicalHistory',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MYRECORDTESTRESULT: {
    name: 'my-record-testresultdetail-testResultId',
    path: '/my-record/testresultdetail/:testResultId',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordTestResult',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_DIAGNOSIS_DETAIL: {
    name: 'my-record-diagnosis-detail',
    path: '/my-record/diagnosis-detail',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordDiagnosisDetail',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_EXAMINATIONS_DETAIL: {
    name: 'my-record-examinations-detail',
    path: '/my-record/examinations-detail',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordExaminationsDetail',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_PROCEDURES_DETAIL: {
    name: 'my-record-procedures-detail',
    path: '/my-record/procedures-detail',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordProceduresDetail',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  MY_RECORD_VISION_TEST_RESULTS_DETAIL: {
    name: 'my-record-test-results-detail',
    path: '/my-record/test-results-detail',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordTestResultsDetail',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  DOCUMENTS: {
    name: 'health-records-gp-medical-record-documents',
    path: '/health-records/gp-medical-record/documents',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'myRecordDocuments',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.GP_MEDICAL_RECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
  },
  DOCUMENT: {
    name: 'health-records-gp-medical-record-documents-id',
    path: '/health-records/gp-medical-record/documents/:id',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.GP_MEDICAL_RECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
  },
  DOCUMENT_DETAIL: {
    name: 'health-records-gp-medical-record-documents-detail-id',
    path: '/health-records/gp-medical-record/documents/detail/:id',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    shouldShowContentHeader: false,
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.GP_MEDICAL_RECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
  },
  NOMINATED_PHARMACY: {
    name: 'nominated-pharmacy',
    path: '/nominated-pharmacy',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_SEARCH: {
    name: 'nominated-pharmacy-search',
    path: '/nominated-pharmacy/search',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CONFIRM: {
    name: 'nominated-pharmacy-confirm',
    path: '/nominated-pharmacy/confirm',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CHANGE_SUCCESS: {
    name: 'nominated-pharmacy-change-success',
    path: '/nominated-pharmacy/change-success',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {},
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_INTERRUPT: {
    name: 'nominated-pharmacy-interrupt',
    path: '/nominated-pharmacy/interrupt',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_DSP_INTERRUPT: {
    name: 'nominated-pharmacy-dsp-interrupt',
    path: '/nominated-pharmacy/dsp-interrupt',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_SEARCH_RESULTS: {
    name: 'nominated-pharmacy-results',
    path: '/nominated-pharmacy/results',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CHECK: {
    name: 'nominated-pharmacy-check',
    path: '/nominated-pharmacy/check',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_CHOOSE_TYPE: {
    name: 'nominated-pharmacy-choose-type',
    path: '/nominated-pharmacy/choose-type',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES: {
    name: 'nominated-pharmacy-online-only-choices',
    path: '/nominated-pharmacy/online-only-choices',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH: {
    name: 'nominated-pharmacy-online-only-search',
    path: '/nominated-pharmacy/online-only-search',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    shouldShowContentHeader: false,
    crumb: {
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}pharmacy/`,
  },
  ORGAN_DONATION: {
    name: 'organ-donation',
    path: '/organ-donation',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_ADDITIONAL_DETAILS: {
    name: 'organ-donation-additional-details',
    path: '/organ-donation/additional-details',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_AMEND: {
    name: 'organ-donation-amend',
    path: '/organ-donation/amend',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_FAITH: {
    name: 'organ-donation-faith',
    path: '/organ-donation/faith',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_MORE_ABOUT_ORGANS: {
    name: 'organ-donation-more-about-organs',
    path: '/organ-donation/more-about-organs',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_SOME_ORGANS: {
    name: 'organ-donation-some-organs',
    path: '/organ-donation/some-organs',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_REVIEW_YOUR_DECISION: {
    name: 'organ-donation-review-your-decision',
    path: '/organ-donation/review-your-decision',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_VIEW_DECISION: {
    name: 'organ-donation-view-decision',
    path: '/organ-donation/view-decision',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_WITHDRAW_REASON: {
    name: 'organ-donation-withdraw-reason',
    path: '/organ-donation/withdraw-reason',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_WITHDRAWN: {
    name: 'organ-donation-withdrawn',
    path: '/organ-donation/withdrawn',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  ORGAN_DONATION_YOUR_CHOICE: {
    name: 'organ-donation-your-choice',
    path: '/organ-donation/your-choice',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'organ_donation',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MORE];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}organ-donation/`,
  },
  GP_MESSAGES: {
    name: 'messages-gp-messages',
    path: '/messages/gp-messages',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessages',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MESSAGES];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_VIEW_ATTACHMENT: {
    name: 'messages-gp-messages-view-attachment',
    path: '/messages/gp-messages/view-attachment',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesViewAttachment',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
          this.allRoutes.GP_MESSAGES_VIEW_MESSAGE,
        ];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_DOWNLOAD_ATTACHMENT: {
    name: 'messages-gp-messages-download-attachment',
    path: '/messages/gp-messages/download-attachment',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesDownloadAttachment',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
          this.allRoutes.GP_MESSAGES_VIEW_MESSAGE,
        ];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_DELETE: {
    name: 'messages-gp-messages-delete',
    path: '/messages/gp-messages/delete',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesDelete',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
          this.allRoutes.GP_MESSAGES_VIEW_MESSAGE,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [
      sjrRedirectRules.im1MessagingDisabledRedirect,
      sjrRedirectRules.deleteMessageRedirect,
    ],
  },
  GP_MESSAGES_URGENCY: {
    name: 'messages-gp-messages-urgency',
    path: '/messages/gp-messages/urgency',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesUrgency',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_URGENCY_CONTACT_GP: {
    name: 'messages-gp-messages-urgency-contact-your-gp',
    path: '/messages/gp-messages/urgency/contact-your-gp',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesUrgencyContactYourGp',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_RECIPIENTS: {
    name: 'messages-gp-messages-recipients',
    path: '/messages/gp-messages/recipients',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesRecipients',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging`,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_VIEW_MESSAGE: {
    name: 'messages-gp-messages-view-details',
    path: '/messages/gp-messages/view-details',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesViewDetails',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_CREATE: {
    name: 'messages-gp-messages-send-message',
    path: '/messages/gp-messages/send-message',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      i18nKey: 'gpMessagesCreate',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
  },
  GP_MESSAGES_DELETE_SUCCESS: {
    name: 'messages-gp-messages-delete-success',
    path: '/messages/gp-messages/delete-success',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/more',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'gpMessagesDeleteSuccess',
      get defaultCrumb() {
        return [
          this.allRoutes.INDEX,
          this.allRoutes.MESSAGES,
          this.allRoutes.GP_MESSAGES,
        ];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}messaging/`,
    sjrRedirectRules: [
      sjrRedirectRules.im1MessagingDisabledRedirect,
      sjrRedirectRules.deleteMessageRedirect,
    ],
  },
  LOGIN_SETTINGS: {
    name: 'account-login-settings',
    path: '/account/login-settings',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'loginSettings',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.ACCOUNT];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/account',
    }],
  },
  LOGIN_SETTINGS_ERROR: {
    name: 'account-login-settings-error',
    path: '/account/login-settings/error',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'loginSettingsError',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.ACCOUNT, this.allRoutes.LOGIN_SETTINGS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/account',
    }],
  },
  LOGIN_BIOMETRIC_ERROR: {
    name: 'biometric-login-error',
    path: '/biometric-login-error',
    isAnonymous: true,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'biometricLoginError',
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      url: '/',
    }],
  },
  PRESCRIPTIONS_VIEW_ORDERS: {
    name: 'prescriptions-view-orders',
    path: '/prescriptions/view-orders',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      nativeDisabled: false,
      i18nKey: 'prescriptions',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    sjrRedirectRules: [sjrRedirectRules.gpAtHandPrescriptionsRedirect],
  },
  PRESCRIPTIONS: {
    name: 'prescriptions',
    path: '/prescriptions',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      nativeDisabled: true,
      i18nKey: 'prescriptions',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    sjrRedirectRules: [sjrRedirectRules.gpAtHandPrescriptionsRedirect],
  },
  PRESCRIPTION_CONFIRM_COURSES: {
    name: 'prescriptions-confirm-prescription-details',
    path: '/prescriptions/confirm-prescription-details',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      i18nKey: 'prescriptionConfirmCourses',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  PRESCRIPTIONS_GP_AT_HAND: {
    name: 'prescriptions-gp-at-hand',
    path: '/prescriptions/gp-at-hand',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      i18nKey: 'prescriptionsGpAtHand',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    sjrRedirectRules: [sjrRedirectRules.im1PrescriptionsRedirect],
  },
  PRE_REGISTRATION_INFORMATION: {
    name: 'pre-registration-information',
    path: '/pre-registration-information',
    isAnonymous: true,
    crumb: {
      i18nKey: 'preRegistrationInformation',
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}app-login/`,
  },
  PRESCRIPTION_REPEAT_COURSES: {
    name: 'prescriptions-repeat-courses',
    path: '/prescriptions/repeat-courses',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      i18nKey: 'prescriptionRepeatCourses',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.PRESCRIPTIONS];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS: {
    name: 'prescriptions-repeat-partial-success',
    path: '/prescriptions/repeat-partial-success',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      nativeDisabled: true,
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    redirectRules: [{
      condition: 'repeatPrescriptionCourses/isOrderPrescriptionInProgress',
      value: false,
      url: '/prescriptions',
    }],
  },
  PRESCRIPTIONS_ORDER_SUCCESS: {
    name: 'prescriptions-order-success',
    path: '/prescriptions/order-success',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/prescriptions',
    crumb: {
      nativeDisabled: true,
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
    redirectRules: [{
      condition: 'repeatPrescriptionCourses/isOrderPrescriptionInProgress',
      value: false,
      url: '/prescriptions',
    }],
  },
  RECALLS: {
    name: 'health-records-gp-medical-record-recalls',
    path: '/health-records/gp-medical-record/recalls',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'recalls',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  REFERRALS: {
    name: 'health-records-gp-medical-record-referrals',
    path: '/health-records/gp-medical-record/referrals',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'referrals',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  HEALTH_CONDITIONS: {
    name: 'health-records-gp-medical-record-health-conditions',
    path: '/health-records/gp-medical-record/health-conditions',
    proofLevel: proofLevel.P9,
    upliftPath: '/uplift/gp-medical-record',
    crumb: {
      i18nKey: 'healthConditions',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  SYMPTOMS: {
    name: 'symptoms',
    path: '/symptoms',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'symptoms',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
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
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'termsAndConditions',
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  LINKED_PROFILES: {
    name: 'linked-profiles',
    path: '/linked-profiles',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.ACCOUNT];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SUMMARY: {
    name: 'linked-profiles-summary',
    path: '/linked-profiles/summary',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.ACCOUNT];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_MORE: {
    name: 'linked-profiles-shutter-more',
    path: '/linked-profiles/shutter/more',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_SYMPTOMS: {
    name: 'linked-profiles-shutter-symptoms',
    path: '/linked-profiles/shutter/symptoms',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_SETTINGS: {
    name: 'linked-profiles-shutter-settings',
    path: '/linked-profiles/shutter/settings',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  LINKED_PROFILES_SHUTTER_APPOINTMENTS: {
    name: 'linked-profiles-shutter-appointments',
    path: '/linked-profiles/shutter/appointments',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS: {
    name: 'linked-profiles-shutter-prescriptions',
    path: '/linked-profiles/shutter/prescriptions',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'linkedProfiles',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  SWITCH_PROFILE: {
    name: 'switch-profile',
    path: '/switch-profile',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'switchProfile',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}proxy/`,
  },
  // Legacy
  LEGACY_MYRECORDWARNING: {
    name: 'my-record-warning',
    path: '/my-record-warning',
    proofLevel: proofLevel.P5,
    crumb: {
      i18nKey: 'legacyMyRecordWarning',
      get defaultCrumb() {
        return [this.allRoutes.INDEX, this.allRoutes.MYRECORD];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
    sjrRedirectRules: [sjrRedirectRules.gpAtHandMyRecordRedirect],
  },
  UPLIFT_APPOINTMENTS: {
    name: 'uplift-appointments',
    path: '/uplift/appointments',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'appointments',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}appointments/`,
  },
  UPLIFT_GP_MEDICAL_RECORD: {
    name: 'uplift-gp-medical-record',
    path: '/uplift/gp-medical-record',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'myRecord',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}record/`,
  },
  UPLIFT_MORE: {
    name: 'uplift-more',
    path: '/uplift/more',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'more',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
  },
  UPLIFT_PRESCRIPTIONS: {
    name: 'uplift-prescriptions',
    path: '/uplift/prescriptions',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'prescriptions',
      get defaultCrumb() {
        return [this.allRoutes.INDEX];
      },
    },
    helpUrl: `${baseNhsAppHelpUrl}prescriptions/`,
  },
  USER_RESEARCH: {
    name: 'user-research',
    path: '/user-research',
    proofLevel: proofLevel.P5,
    crumb: {
      nativeDisabled: true,
      i18nKey: 'userResearch',
      get defaultCrumb() {
        return [];
      },
    },
    helpUrl: baseNhsAppHelpUrl,
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
  [routes.ACCOUNT_COOKIES.name]: {
    ignoreStore: true,
    defaultPath: routes.ACCOUNT.path,
  },
  [routes.APPOINTMENT_BOOKING_SUCCESS.name]: {
    ignoreStore: true,
    defaultPath: routes.APPOINTMENTS.path,
  },
  [routes.GP_APPOINTMENTS.name]: {
    ignoreStore: true,
    defaultPath: routes.APPOINTMENTS.path,
  },
  [routes.APPOINTMENT_CANCELLING_SUCCESS.name]: {
    ignoreStore: true,
    defaultPath: routes.APPOINTMENTS.path,
  },
  [routes.LINKED_PROFILES_SHUTTER_APPOINTMENTS.name]: {
    ignoreStore: true,
    defaultPath: routes.APPOINTMENTS.path,
  },
  [routes.LINKED_PROFILES_SHUTTER_PRESCRIPTIONS.name]: {
    ignoreStore: true,
    defaultPath: routes.INDEX.path,
  },
  [routes.ORGAN_DONATION.name]: {
    defaultPath: routes.MORE.path,
  },
  [routes.ORGAN_DONATION_VIEW_DECISION.name]: {
    defaultPath: routes.MORE.path,
  },
  [routes.GP_MESSAGES.name]: {
    ignoreStore: true,
    defaultPath: routes.MESSAGES.path,
  },
  [routes.GP_MESSAGES_URGENCY.name]: {
    defaultPath: routes.GP_MESSAGES.path,
  },
  [routes.GP_MESSAGES_VIEW_MESSAGE.name]: {
    ignoreStore: true,
    defaultPath: routes.GP_MESSAGES.path,
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

export const findByPage = (page) => {
  switch (page) {
    case AppPage.HOME_PAGE:
      return routes.INDEX;
    case AppPage.APPOINTMENTS:
      return routes.APPOINTMENTS;
    case AppPage.PRESCRIPTIONS:
      return routes.PRESCRIPTIONS;
    case AppPage.HEALTH_RECORDS:
      return routes.HEALTH_RECORDS;
    case AppPage.SYMPTOMS:
      return routes.SYMPTOMS;
    case AppPage.MESSAGES:
      return routes.MESSAGES;
    default:
      consola.error(new Error(`Navigation for page enum value not implemented: ${page}`));
      return undefined;
  }
};

export const getRouteNames = () => Object.keys(routes).map(key => routes[key].name);

export const REDIRECT_PAGE_PARAMETER = 'redirect_to_page';
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

export const {
  ACCOUNT,
  ACCOUNT_COOKIES,
  ACCOUNT_NOTIFICATIONS,
  ACCOUNT_SIGNOUT,
  ACUTE_MEDICINES,
  ALLERGIESANDREACTIONS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CANCELLING_SUCCESS,
  APPOINTMENT_CANCEL_NOJS,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENT_BOOKING_SUCCESS,
  APPOINTMENT_ADD_TO_CALENDAR,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_GP_ADVICE,
  APPOINTMENT_GP_AT_HAND,
  APPOINTMENT_INFORMATICA,
  APPOINTMENTS,
  AUTH_RETURN,
  BEGINLOGIN,
  CHECKYOURSYMPTOMS,
  CONSULTATIONS,
  CURRENT_MEDICINES,
  DATA_SHARING_OVERVIEW,
  DATA_SHARING_WHERE_USED,
  DATA_SHARING_DOES_NOT_APPLY,
  DATA_SHARING_MAKE_YOUR_CHOICE,
  DIAGNOSIS_V2,
  DISCONTINUED_MEDICINES,
  DOCUMENT,
  DOCUMENT_DETAIL,
  DOCUMENTS,
  ENCOUNTERS,
  EVENTS,
  EXAMINATIONS_V2,
  GP_APPOINTMENTS,
  GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
  HEALTH_CONDITIONS,
  HOSPITAL_APPOINTMENTS,
  INDEX,
  IMMUNISATIONS,
  INTERSTITIAL_REDIRECTOR,
  LEGACY_MYRECORDWARNING,
  LINKED_PROFILES,
  LINKED_PROFILES_SUMMARY,
  LINKED_PROFILES_SHUTTER_MORE,
  LINKED_PROFILES_SHUTTER_SYMPTOMS,
  LINKED_PROFILES_SHUTTER_SETTINGS,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS,
  LOGIN,
  LOGOUT,
  MEDICINES,
  MEDICAL_HISTORY,
  MESSAGES,
  HEALTH_INFORMATION_UPDATES,
  HEALTH_INFORMATION_UPDATES_MESSAGES,
  MORE,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  HEALTH_RECORDS,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
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
  GP_MESSAGES,
  GP_MESSAGES_URGENCY,
  GP_MESSAGES_URGENCY_CONTACT_GP,
  GP_MESSAGES_RECIPIENTS,
  GP_MESSAGES_VIEW_MESSAGE,
  GP_MESSAGES_CREATE,
  GP_MESSAGES_DOWNLOAD_ATTACHMENT,
  GP_MESSAGES_DELETE,
  GP_MESSAGES_DELETE_SUCCESS,
  GP_MESSAGES_VIEW_ATTACHMENT,
  LOGIN_SETTINGS,
  LOGIN_SETTINGS_ERROR,
  LOGIN_BIOMETRIC_ERROR,
  PRESCRIPTIONS_VIEW_ORDERS,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
  PRE_REGISTRATION_INFORMATION,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS,
  PRESCRIPTIONS_ORDER_SUCCESS,
  PROCEDURES_V2,
  RECALLS,
  REFERRALS,
  SWITCH_PROFILE,
  SYMPTOMS,
  TERMSANDCONDITIONS,
  TESTRESULTID,
  TESTRESULTS,
  TESTRESULTSDETAIL,
  UPLIFT_APPOINTMENTS,
  UPLIFT_GP_MEDICAL_RECORD,
  UPLIFT_MORE,
  UPLIFT_PRESCRIPTIONS,
  USER_RESEARCH,
} = routes;

export default routes;
