import wayfinderPage from '@/pages/wayfinder';

import breadcrumbs from '@/breadcrumbs/wayfinder';

import { WAYFINDER_PATH } from '@/router/paths';
import { WAYFINDER_NAME } from '@/router/names';
import { UPLIFT_APPOINTMENTS } from '@/router/routes/appointments';
import { HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH } from '@/router/externalLinks';

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
    helpPath: HOSPITAL_AND_OTHER_APPOINTMENTS_HELP_PATH,
    sjrRedirectRules: [sjrRedirectRules.wayfinderAppointmentsDisabledRedirect],
    nativeNavigation: APPOINTMENTS_MENU_ITEM,
  },
};

export default [
  WAYFINDER_APPOINTMENTS,
];
