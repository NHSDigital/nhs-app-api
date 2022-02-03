import LoginPage from '@/pages/Login';
import LoginBiometricErrorPage from '@/pages/biometric-login-error';
import PreRegistrationInformationPage from '@/pages/pre-registration-information/index';
import AuthReturnPage from '@/pages/auth-return/index';
import TermsAndConditionsPage from '@/pages/terms-and-conditions';
import UserResearchPage from '@/pages/user-research';
import Notifications from '@/pages/notifications/index';
import NotificationsGenericError from '@/pages/notifications/notifications-generic-error';

import {
  AUTH_RETURN_PATH,
  BEGINLOGIN_PATH,
  LOGIN_BIOMETRIC_ERROR_PATH,
  LOGIN_PATH,
  NOTIFICATIONS_GENERIC_FAILURE_PATH,
  NOTIFICATIONS_PATH,
  PRE_REGISTRATION_INFORMATION_PATH,
  TERMSANDCONDITIONS_PATH,
  USER_RESEARCH_PATH,
} from '@/router/paths';
import {
  AUTH_RETURN_NAME,
  BEGINLOGIN_NAME,
  LOGIN_BIOMETRIC_ERROR_NAME,
  LOGIN_NAME,
  NOTIFICATIONS_GENERIC_FAILURE_NAME,
  NOTIFICATIONS_NAME,
  PRE_REGISTRATION_INFORMATION_NAME,
  TERMSANDCONDITIONS_NAME,
  USER_RESEARCH_NAME,
} from '@/router/names';

import viewedPreRegInstructions from '@/middleware/viewedPreRegInstructions';
import notificationsPrompt from '@/middleware/notificationsPrompt';
import nativeLogout from '@/middleware/nativeLogout';
import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import { APP_LOGIN_HELP_PATH } from '@/router/externalLinks';
import proofLevel from '@/lib/proofLevel';

const getTermsConditionTitle = (store, i18n) => (
  i18n.t(store.state.termsAndConditions.updatedConsentRequired
    ? 'termsAndConditions.updated.title'
    : 'navigation.pages.headers.termsAndConditions')
);

export const LOGIN = {
  path: LOGIN_PATH,
  name: LOGIN_NAME,
  component: LoginPage,
  meta: {
    isAnonymous: true,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    shouldShowContentHeader: false,
    helpPath: APP_LOGIN_HELP_PATH,
    middleware: [nativeLogout],
  },
};

export const LOGIN_BIOMETRIC_ERROR = {
  path: LOGIN_BIOMETRIC_ERROR_PATH,
  name: LOGIN_BIOMETRIC_ERROR_NAME,
  component: LoginBiometricErrorPage,
  meta: {
    headerKey: 'navigation.pages.headers.loginBiometricError',
    titleKey: 'navigation.pages.titles.loginBiometricError',
    isAnonymous: true,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpPath: APP_LOGIN_HELP_PATH,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: LOGIN,
    }],
  },
};

export const PRE_REGISTRATION = {
  path: PRE_REGISTRATION_INFORMATION_PATH,
  name: PRE_REGISTRATION_INFORMATION_NAME,
  component: PreRegistrationInformationPage,
  meta: {
    headerKey: 'navigation.pages.headers.preRegistrationInformation',
    titleKey: 'navigation.pages.titles.preRegistrationInformation',
    isAnonymous: true,
    middleware: [viewedPreRegInstructions],
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpPath: APP_LOGIN_HELP_PATH,
  },
};

export const BEGIN_LOGIN = {
  path: BEGINLOGIN_PATH,
  name: BEGINLOGIN_NAME,
  meta: {
    isAnonymous: true,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpPath: APP_LOGIN_HELP_PATH,
  },
};

export const AUTH_RETURN = {
  path: AUTH_RETURN_PATH,
  name: AUTH_RETURN_NAME,
  component: AuthReturnPage,
  meta: {
    isAnonymous: true,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpPath: APP_LOGIN_HELP_PATH,
  },
};

export const TERMS_AND_CONDITIONS = {
  path: TERMSANDCONDITIONS_PATH,
  name: TERMSANDCONDITIONS_NAME,
  component: TermsAndConditionsPage,
  meta: {
    headerKey: getTermsConditionTitle,
    titleKey: getTermsConditionTitle,
    proofLevel: proofLevel.P5,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const USER_RESEARCH = {
  path: USER_RESEARCH_PATH,
  name: USER_RESEARCH_NAME,
  component: UserResearchPage,
  meta: {
    headerKey: 'navigation.pages.headers.userResearch',
    titleKey: 'navigation.pages.titles.userResearch',
    proofLevel: proofLevel.P5,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const NOTIFICATIONS = {
  path: NOTIFICATIONS_PATH,
  name: NOTIFICATIONS_NAME,
  component: Notifications,
  meta: {
    headerKey: 'navigation.pages.headers.notifications',
    titleKey: 'navigation.pages.headers.notifications',
    middleware: [notificationsPrompt],
    proofLevel: proofLevel.P5,
    crumb: {},
  },
};

export const NOTIFICATIONS_GENERIC_ERROR = {
  path: NOTIFICATIONS_GENERIC_FAILURE_PATH,
  name: NOTIFICATIONS_GENERIC_FAILURE_NAME,
  component: NotificationsGenericError,
  meta: {
    headerKey: 'navigation.pages.headers.notificationsGenericError',
    titleKey: 'navigation.pages.headers.notificationsGenericError',
    proofLevel: proofLevel.P5,
    crumb: {},
  },
};

export default [
  LOGIN,
  LOGIN_BIOMETRIC_ERROR,
  PRE_REGISTRATION,
  BEGIN_LOGIN,
  AUTH_RETURN,
  TERMS_AND_CONDITIONS,
  USER_RESEARCH,
  NOTIFICATIONS,
  NOTIFICATIONS_GENERIC_ERROR,
];
