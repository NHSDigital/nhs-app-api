import AddToCalendarInterruptPage from '@/pages/appointments/gp-appointments/add-to-calendar-interrupt';
import AdminHelpPage from '@/pages/appointments/gp-appointments/admin-help';
import AppointmentsIndexPage from '@/pages/appointments';
import BookingGuidancePage from '@/pages/appointments/gp-appointments/booking-guidance';
import BookingPage from '@/pages/appointments/gp-appointments/booking';
import BookingSuccessPage from '@/pages/appointments/gp-appointments/booking-success';
import CancellingPage from '@/pages/appointments/gp-appointments/cancelling';
import CancellingSuccessPage from '@/pages/appointments/gp-appointments/cancelling-success';
import ConfirmationPage from '@/pages/appointments/gp-appointments/confirmation';
import GpAdvicePage from '@/pages/appointments/gp-appointments/gp-advice';
import GpAppointmentsIndexPage from '@/pages/appointments/gp-appointments';
import GpAtHandPage from '@/pages/appointments/gp-at-hand';
import HospitalAppointmentsPage from '@/pages/appointments/hospital-appointments';
import InformaticaPage from '@/pages/appointments/informatica';
import UpliftAppointmentsPage from '@/pages/uplift/appointments';

import breadcrumbs from '@/breadcrumbs/appointments';
import {
  UPLIFT_APPOINTMENTS_PATH,
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
  HOSPITAL_APPOINTMENTS_PATH,
  APPOINTMENT_GP_AT_HAND_PATH,
  APPOINTMENT_INFORMATICA_PATH,
  APPOINTMENT_BOOKING_GUIDANCE_PATH,
  APPOINTMENT_BOOKING_PATH,
  APPOINTMENT_CONFIRMATIONS_PATH,
  APPOINTMENT_BOOKING_SUCCESS_PATH,
  APPOINTMENT_ADD_TO_CALENDAR_PATH,
  APPOINTMENT_CANCELLING_PATH,
  APPOINTMENT_CANCELLING_SUCCESS_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  APPOINTMENT_GP_ADVICE_PATH,
} from '@/router/paths';
import {
  UPLIFT_APPOINTMENTS_NAME,
  APPOINTMENTS_NAME,
  GP_APPOINTMENTS_NAME,
  HOSPITAL_APPOINTMENTS_NAME,
  APPOINTMENT_GP_AT_HAND_NAME,
  APPOINTMENT_INFORMATICA_NAME,
  APPOINTMENT_BOOKING_GUIDANCE_NAME,
  APPOINTMENT_BOOKING_NAME,
  APPOINTMENT_CONFIRMATIONS_NAME,
  APPOINTMENT_BOOKING_SUCCESS_NAME,
  APPOINTMENT_ADD_TO_CALENDAR_NAME,
  APPOINTMENT_CANCELLING_NAME,
  APPOINTMENT_CANCELLING_SUCCESS_NAME,
  APPOINTMENT_ADMIN_HELP_NAME,
  APPOINTMENT_GP_ADVICE_NAME,
} from '@/router/names';

import { APPOINTMENTS_MENU_ITEM } from '@/middleware/nativeNavigation';
import urlResolution from '@/middleware/urlResolution';

import proofLevel from '@/lib/proofLevel';
import { appointmentsHelpUrl, onlineConsultationsHelpUrl } from '@/router/externalLinks';
import sjrRedirectRules from '@/router/sjrRedirectRules';
import get from 'lodash/fp/get';

export const UPLIFT_APPOINTMENTS = {
  path: UPLIFT_APPOINTMENTS_PATH,
  name: UPLIFT_APPOINTMENTS_NAME,
  component: UpliftAppointmentsPage,
  meta: {
    headerKey: 'pageHeaders.appointments',
    titleKey: 'pageTitles.appointments',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.UPLIFT_APPOINTMENTS_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
    middleware: [urlResolution],
  },
};

export const APPOINTMENTS = {
  path: APPOINTMENTS_PATH,
  name: APPOINTMENTS_NAME,
  component: AppointmentsIndexPage,
  meta: {
    headerKey: 'pageHeaders.appointments',
    titleKey: 'pageTitles.appointments',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.APPOINTMENTS_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const GP_APPOINTMENTS = {
  path: GP_APPOINTMENTS_PATH,
  name: GP_APPOINTMENTS_NAME,
  component: GpAppointmentsIndexPage,
  meta: {
    headerKey: 'pageHeaders.gpAppointments',
    titleKey: 'pageTitles.gpAppointments',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.GP_APPOINTMENTS_CRUMB,
    proxyShutterPath: 'linked-profiles/shutter/appointments',
    helpUrl: appointmentsHelpUrl,
    sjrRedirectRules: [
      sjrRedirectRules.linkedAccountAppointmentRedirect,
      sjrRedirectRules.gpAtHandAppointmentRedirect,
      sjrRedirectRules.informaticaAppointmentRedirect,
    ],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const HOSPITAL_APPOINTMENTS = {
  path: HOSPITAL_APPOINTMENTS_PATH,
  name: HOSPITAL_APPOINTMENTS_NAME,
  component: HospitalAppointmentsPage,
  meta: {
    headerKey: 'pageHeaders.hospitalAppointments',
    titleKey: 'pageTitles.hospitalAppointments',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.HOSPITAL_APPOINTMENTS_CRUMB,
    helpUrl: appointmentsHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.silverIntegrationsSecondaryAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const GP_AT_HAND = {
  path: APPOINTMENT_GP_AT_HAND_PATH,
  name: APPOINTMENT_GP_AT_HAND_NAME,
  component: GpAtHandPage,
  meta: {
    headerKey: 'pageHeaders.serviceUnavailable',
    titleKey: 'pageTitles.serviceUnavailable',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.GP_AT_HAND_CRUMB,
    helpUrl: appointmentsHelpUrl,
    sjrRedirectRules: [
      sjrRedirectRules.im1AppointmentRedirect,
      sjrRedirectRules.informaticaAppointmentRedirect,
    ],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const INFORMATICA = {
  path: APPOINTMENT_INFORMATICA_PATH,
  name: APPOINTMENT_INFORMATICA_NAME,
  component: InformaticaPage,
  meta: {
    headerKey: 'pageHeaders.serviceUnavailable',
    titleKey: 'pageTitles.serviceUnavailable',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.INFORMATICA_CRUMB,
    helpUrl: appointmentsHelpUrl,
    sjrRedirectRules: [
      sjrRedirectRules.gpAtHandAppointmentRedirect,
      sjrRedirectRules.im1AppointmentRedirect,
    ],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const BOOKING_GUIDANCE = {
  path: APPOINTMENT_BOOKING_GUIDANCE_PATH,
  name: APPOINTMENT_BOOKING_GUIDANCE_NAME,
  component: BookingGuidancePage,
  meta: {
    headerKey: 'pageHeaders.appointmentGuidance',
    titleKey: 'pageTitles.appointmentGuidance',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.BOOKING_GUIDANCE_CRUMB,
    helpUrl: appointmentsHelpUrl,
    sjrRedirectRules: [
      sjrRedirectRules.gpAtHandAppointmentRedirect,
      sjrRedirectRules.informaticaAppointmentRedirect,
    ],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const BOOKING = {
  path: APPOINTMENT_BOOKING_PATH,
  name: APPOINTMENT_BOOKING_NAME,
  component: BookingPage,
  meta: {
    headerKey: 'pageHeaders.appointmentBooking',
    titleKey: 'pageTitles.appointmentBooking',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.BOOKING_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const CONFIRMATION = {
  path: APPOINTMENT_CONFIRMATIONS_PATH,
  name: APPOINTMENT_CONFIRMATIONS_NAME,
  component: ConfirmationPage,
  meta: {
    headerKey: 'pageHeaders.appointmentConfirmation',
    titleKey: 'pageTitles.appointmentConfirmation',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.CONFIRMATION_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const BOOKING_SUCCESS = {
  path: APPOINTMENT_BOOKING_SUCCESS_PATH,
  name: APPOINTMENT_BOOKING_SUCCESS_NAME,
  component: BookingSuccessPage,
  meta: {
    headerKey: (store, i18n) => {
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        return i18n.t('pageHeaders.appointmentProxyBookingSuccess', { name: givenName });
      }
      return i18n.t('pageHeaders.appointmentBookingSuccess');
    },
    titleKey: (store, i18n) => {
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        return i18n.t('pageTitles.appointmentProxyBookingSuccess', { name: givenName });
      }
      return i18n.t('pageTitles.appointmentBookingSuccess');
    },
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.BOOKING_SUCCESS_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
    redirectRules: [{
      condition: 'availableAppointments/isBookingAppointmentInProgress',
      value: false,
      route: GP_APPOINTMENTS,
    }],
  },
};

export const ADD_TO_CALENDAR = {
  path: APPOINTMENT_ADD_TO_CALENDAR_PATH,
  name: APPOINTMENT_ADD_TO_CALENDAR_NAME,
  component: AddToCalendarInterruptPage,
  meta: {
    titleKey: 'pageTitles.appointmentAddToCalendar',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.ADD_TO_CALENDAR_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const CANCELLING = {
  path: APPOINTMENT_CANCELLING_PATH,
  name: APPOINTMENT_CANCELLING_NAME,
  component: CancellingPage,
  meta: {
    headerKey: 'pageHeaders.appointmentCancelling',
    titleKey: 'pageTitles.appointmentCancelling',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.CANCELLING_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const CANCELLING_SUCCESS = {
  path: APPOINTMENT_CANCELLING_SUCCESS_PATH,
  name: APPOINTMENT_CANCELLING_SUCCESS_NAME,
  component: CancellingSuccessPage,
  meta: {
    headerKey: (store, i18n) => {
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        return i18n.t('pageHeaders.appointmentProxyCancellingSuccess', { name: givenName });
      }
      return i18n.t('pageHeaders.appointmentCancellingSuccess');
    },
    titleKey: (store, i18n) => {
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        return i18n.t('pageTitles.appointmentProxyCancellingSuccess', { name: givenName });
      }
      return i18n.t('pageTitles.appointmentCancellingSuccess');
    },
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.CANCELLING_SUCCESS_CRUMB,
    helpUrl: appointmentsHelpUrl,
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
    redirectRules: [{
      condition: 'myAppointments/isCancellingAppointmentInProgress',
      value: false,
      route: GP_APPOINTMENTS,
    }],
  },
};

export const ADMIN_HELP = {
  path: APPOINTMENT_ADMIN_HELP_PATH,
  name: APPOINTMENT_ADMIN_HELP_NAME,
  component: AdminHelpPage,
  meta: {
    headerKey: 'pageHeaders.appointmentAdminHelp',
    titleKey: 'pageTitles.appointmentAdminHelp',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.ADMIN_HELP_CRUMB,
    helpUrl: onlineConsultationsHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.adminHelpDisabledRedirect],
    warningBanner: true,
  },
};

export const GP_ADVICE = {
  path: APPOINTMENT_GP_ADVICE_PATH,
  name: APPOINTMENT_GP_ADVICE_NAME,
  component: GpAdvicePage,
  meta: {
    headerKey: 'pageHeaders.appointmentGpAdvice',
    titleKey: 'pageTitles.appointmentGpAdvice',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.GP_ADVICE_CRUMB,
    helpUrl: onlineConsultationsHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.gpAdviceDisabledRedirect],
    warningBanner: true,
  },
};

export default [
  UPLIFT_APPOINTMENTS,
  APPOINTMENTS,
  GP_APPOINTMENTS,
  HOSPITAL_APPOINTMENTS,
  GP_AT_HAND,
  INFORMATICA,
  BOOKING_GUIDANCE,
  BOOKING,
  CONFIRMATION,
  BOOKING_SUCCESS,
  ADD_TO_CALENDAR,
  CANCELLING,
  CANCELLING_SUCCESS,
  ADMIN_HELP,
  GP_ADVICE,
];
