import get from 'lodash/fp/get';

import PrescriptionIndexPage from '@/pages/prescriptions';
import ViewPrescriptionOrdersPage from '@/pages/prescriptions/view-orders';
import RepeatCoursesPage from '@/pages/prescriptions/repeat-courses';
import ConfirmPrescriptionDetailsPage from '@/pages/prescriptions/confirm-prescription-details';
import PrescriptionOrderSuccessPage from '@/pages/prescriptions/order-success';
import PrescriptionPartialOrderSuccessPage from '@/pages/prescriptions/repeat-partial-success';
import PrescriptionsGpAtHand from '@/pages/prescriptions/gp-at-hand';
import UpliftPrescriptionsPage from '@/pages/uplift/prescriptions';

import proofLevel from '@/lib/proofLevel';

import { prescriptionsHelpUrl } from '@/router/externalLinks';
import sjrRedirectRules from '@/router/sjrRedirectRules';

import {
  PRESCRIPTIONS_PATH,
  PRESCRIPTIONS_VIEW_ORDERS_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTIONS_ORDER_SUCCESS_PATH,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_PATH,
  PRESCRIPTIONS_GP_AT_HAND_PATH,
  UPLIFT_PRESCRIPTIONS_PATH,
} from '@/router/paths';
import {
  PRESCRIPTIONS_NAME,
  PRESCRIPTIONS_VIEW_ORDERS_NAME,
  PRESCRIPTION_REPEAT_COURSES_NAME,
  PRESCRIPTION_CONFIRM_COURSES_NAME,
  PRESCRIPTIONS_ORDER_SUCCESS_NAME,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_NAME,
  PRESCRIPTIONS_GP_AT_HAND_NAME,
  UPLIFT_PRESCRIPTIONS_NAME,
} from '@/router/names';

import breadcrumbs from '@/breadcrumbs/prescriptions';

import { PRESCRIPTIONS_MENU_ITEM } from '@/middleware/nativeNavigation';
import urlResolution from '@/middleware/urlResolution';

export const UPLIFT_PRESCRIPTIONS = {
  name: UPLIFT_PRESCRIPTIONS_NAME,
  path: UPLIFT_PRESCRIPTIONS_PATH,
  component: UpliftPrescriptionsPage,
  meta: {
    headerKey: 'pageHeaders.prescriptions',
    titleKey: 'pageTitles.prescriptions',
    crumb: breadcrumbs.UPLIFT_PRESCRIPTIONS_CRUMB,
    nativeDisabled: true,
    i18nKey: 'prescriptions',
    proofLevel: proofLevel.P5,
    helpUrl: prescriptionsHelpUrl,
    middleware: [urlResolution],
  },
};

export const PRESCRIPTIONS = {
  path: PRESCRIPTIONS_PATH,
  name: PRESCRIPTIONS_NAME,
  component: PrescriptionIndexPage,
  meta: {
    headerKey: 'pageHeaders.prescriptions',
    titleKey: 'pageTitles.prescriptions',
    crumb: breadcrumbs.PRESCRIPTIONS_CRUMB,
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    helpUrl: prescriptionsHelpUrl,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    sjrRedirectRules: [sjrRedirectRules.gpAtHandPrescriptionsRedirect],
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const VIEW_ORDERS = {
  path: PRESCRIPTIONS_VIEW_ORDERS_PATH,
  name: PRESCRIPTIONS_VIEW_ORDERS_NAME,
  component: ViewPrescriptionOrdersPage,
  meta: {
    headerKey: 'pageHeaders.viewPrescriptionsOrder',
    titleKey: 'pageTitles.viewPrescriptionsOrder',
    crumb: breadcrumbs.PRESCRIPTIONS_VIEW_ORDERS_CRUMB,
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    helpUrl: prescriptionsHelpUrl,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    sjrRedirectRules: [sjrRedirectRules.gpAtHandPrescriptionsRedirect],
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const REPEAT_COURSES = {
  path: PRESCRIPTION_REPEAT_COURSES_PATH,
  name: PRESCRIPTION_REPEAT_COURSES_NAME,
  component: RepeatCoursesPage,
  meta: {
    headerKey: 'pageHeaders.repeatPrescriptionCourses',
    titleKey: 'pageTitles.repeatPrescriptionCourses',
    crumb: breadcrumbs.PRESCRIPTION_REPEAT_COURSES_CRUMB,
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    helpUrl: prescriptionsHelpUrl,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const CONFIRM_DETAILS = {
  path: PRESCRIPTION_CONFIRM_COURSES_PATH,
  name: PRESCRIPTION_CONFIRM_COURSES_NAME,
  component: ConfirmPrescriptionDetailsPage,
  meta: {
    headerKey: 'pageHeaders.confirmPrescription',
    titleKey: 'pageTitles.confirmPrescription',
    crumb: breadcrumbs.PRESCRIPTION_CONFIRM_COURSES_CRUMB,
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    helpUrl: prescriptionsHelpUrl,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
  },
};

export const ORDER_SUCCESS = {
  path: PRESCRIPTIONS_ORDER_SUCCESS_PATH,
  name: PRESCRIPTIONS_ORDER_SUCCESS_NAME,
  component: PrescriptionOrderSuccessPage,
  meta: {
    headerKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return store.getters['session/isProxying']
        ? i18n.t('pageHeaders.prescriptionProxyOrderSuccess', { name: givenName })
        : i18n.t('pageHeaders.prescriptionOrderSuccess');
    },
    titleKey: (store, i18n) => {
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      return store.getters['session/isProxying']
        ? i18n.t('pageTitles.prescriptionProxyOrderSuccess', { name: givenName })
        : i18n.t('pageTitles.prescriptionOrderSuccess');
    },
    crumb: breadcrumbs.PRESCRIPTION_ORDER_SUCCESS_CRUMB,
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    helpUrl: prescriptionsHelpUrl,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
    redirectRules: [{
      condition: 'repeatPrescriptionCourses/isOrderPrescriptionInProgress',
      value: false,
      route: PRESCRIPTIONS,
    }],
  },
};

export const REPEAT_PARTIAL_SUCCESS = {
  path: PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_PATH,
  name: PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_NAME,
  component: PrescriptionPartialOrderSuccessPage,
  meta: {
    headerKey: 'pageHeaders.repeatPrescriptionsPartialSuccess',
    titleKey: 'pageTitles.repeatPrescriptionsPartialSuccess',
    crumb: breadcrumbs.PRESCRIPTION_REPEAT_PARTIAL_SUCCESS_CRUMB,
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    helpUrl: prescriptionsHelpUrl,
    proxyShutterPath: '/linked-profiles/shutter/prescriptions',
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
    redirectRules: [{
      condition: 'repeatPrescriptionCourses/isOrderPrescriptionInProgress',
      value: false,
      route: PRESCRIPTIONS,
    }],
  },
};

export const PRESCRIPTIONS_GP_AT_HAND = {
  path: PRESCRIPTIONS_GP_AT_HAND_PATH,
  name: PRESCRIPTIONS_GP_AT_HAND_NAME,
  component: PrescriptionsGpAtHand,
  meta: {
    headerKey: 'pageHeaders.serviceUnavailable',
    titleKey: 'pageTitles.serviceUnavailable',
    upliftRoute: UPLIFT_PRESCRIPTIONS,
    crumb: breadcrumbs.PRESCRIPTIONS_GP_AT_HAND_CRUMB,
    proofLevel: proofLevel.P9,
    helpUrl: prescriptionsHelpUrl,
    nativeNavigation: PRESCRIPTIONS_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.im1PrescriptionsRedirect],
  },
};

export default [
  UPLIFT_PRESCRIPTIONS,
  PRESCRIPTIONS,
  VIEW_ORDERS,
  REPEAT_COURSES,
  CONFIRM_DETAILS,
  ORDER_SUCCESS,
  REPEAT_PARTIAL_SUCCESS,
  PRESCRIPTIONS_GP_AT_HAND,
];
