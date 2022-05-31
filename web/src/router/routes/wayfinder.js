import wayfinderPage from '@/pages/wayfinder';
import referralsOrAppointmentsHelpPage from '@/pages/wayfinder/help/referrals-or-appointments-help';
import confirmedAppointmentsHelpPage from '@/pages/wayfinder/help/confirmed-appointments-help';
import referralsInReviewPage from '@/pages/wayfinder/help/referrals-in-review-help';

import breadcrumbs from '@/breadcrumbs/wayfinder';

import {
  WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_PATH,
  WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_PATH,
  WAYFINDER_REFERRALS_IN_REVIEW_HELP_PATH,
  WAYFINDER_PATH,
} from '@/router/paths';
import {
  WAYFINDER_NAME,
  WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_NAME,
  WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_NAME,
  WAYFINDER_REFERRALS_IN_REVIEW_HELP_NAME,
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

export const REFERRALS_OR_APPOINTMENTS_HELP = {
  path: WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_PATH,
  name: WAYFINDER_REFERRALS_OR_APPOINTMENTS_HELP_NAME,
  component: referralsOrAppointmentsHelpPage,
  meta: {
    headerKey: 'navigation.pages.headers.referralsOrAppointmentsHelp',
    titleKey: 'navigation.pages.titles.referralsOrAppointmentsHelp',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.WAYFINDER_HELP_CRUMB,
    helpPath: REFERRALS_HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH,
    sjrRedirectRules: [sjrRedirectRules.wayfinderAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const CONFIRMED_APPOINTMENTS_HELP = {
  path: WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_PATH,
  name: WAYFINDER_CONFIRMED_APPOINTMENTS_HELP_NAME,
  component: confirmedAppointmentsHelpPage,
  meta: {
    headerKey: 'navigation.pages.headers.confirmedAppointmentsHelp',
    titleKey: 'navigation.pages.titles.confirmedAppointmentsHelp',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.WAYFINDER_HELP_CRUMB,
    helpPath: REFERRALS_HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH,
    sjrRedirectRules: [sjrRedirectRules.wayfinderAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export const REFERRALS_IN_REVIEW_HELP = {
  path: WAYFINDER_REFERRALS_IN_REVIEW_HELP_PATH,
  name: WAYFINDER_REFERRALS_IN_REVIEW_HELP_NAME,
  component: referralsInReviewPage,
  meta: {
    headerKey: 'navigation.pages.headers.referralsInReviewHelp',
    titleKey: 'navigation.pages.titles.referralsInReviewHelp',
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
  REFERRALS_OR_APPOINTMENTS_HELP,
  CONFIRMED_APPOINTMENTS_HELP,
  REFERRALS_IN_REVIEW_HELP,
];
