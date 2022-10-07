import wayfinderPage from '@/pages/appointments/hospital-referrals-appointments/hospital-referrals-appointments';
import wayfinderHelpPage from '@/pages/appointments/hospital-referrals-appointments/help';
import wayfinderWaitingLists from '@/pages/appointments/hospital-referrals-appointments/waiting-lists';

import breadcrumbs from '@/breadcrumbs/wayfinder';

import {
  WAYFINDER_PATH,
  WAYFINDER_HELP_PATH,
  WAYFINDER_WAITING_LISTS_PATH,
} from '@/router/paths';
import {
  WAYFINDER_NAME,
  WAYFINDER_HELP_NAME,
  WAYFINDER_WAITING_LISTS_NAME,
} from '@/router/names';

import { UPLIFT_APPOINTMENTS } from '@/router/routes/appointments';
import { REFERRALS_HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH } from '@/router/externalLinks';

import { APPOINTMENTS_MENU_ITEM } from '@/middleware/nativeNavigation';

import proofLevel from '@/lib/proofLevel';
import sjrRedirectRules from '@/router/sjrRedirectRules';

export const WAYFINDER_APPOINTMENTS = {
  path: WAYFINDER_PATH,
  name: WAYFINDER_NAME,
  component: wayfinderPage,
  meta: {
    headerKey: 'navigation.pages.headers.wayfinder',
    titleKey: 'navigation.pages.titles.wayfinder',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.APPOINTMENTS_WAYFINDER_CRUMB,
    helpPath: REFERRALS_HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH,
    sjrRedirectRules: [sjrRedirectRules.wayfinderAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const WAYFINDER_WAITING_LISTS = {
  path: WAYFINDER_WAITING_LISTS_PATH,
  name: WAYFINDER_WAITING_LISTS_NAME,
  component: wayfinderWaitingLists,
  meta: {
    headerKey: 'navigation.pages.headers.waitTimes',
    titleKey: 'navigation.pages.titles.waitTimes',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.WAYFINDER_WAIT_TIMES_CRUMB,
    helpPath: REFERRALS_HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH,
    sjrRedirectRules: [sjrRedirectRules.wayfinderAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const WAYFINDER_HELP = {
  path: WAYFINDER_HELP_PATH,
  name: WAYFINDER_HELP_NAME,
  component: wayfinderHelpPage,
  meta: {
    headerKey: 'navigation.pages.headers.wayfinderHelp',
    titleKey: 'navigation.pages.titles.wayfinderHelp',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.WAYFINDER_HELP_CRUMB,
    helpPath: REFERRALS_HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH,
    sjrRedirectRules: [sjrRedirectRules.wayfinderAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export default [
  WAYFINDER_APPOINTMENTS,
  WAYFINDER_HELP,
  WAYFINDER_WAITING_LISTS,
];
