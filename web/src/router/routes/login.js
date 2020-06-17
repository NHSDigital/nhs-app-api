import LoginPage from '@/pages/Login';
import LoginBiometricErrorPage from '@/pages/biometric-login-error';
import PreRegistrationInformationPage from '@/pages/pre-registration-information/index';
import AuthReturnPage from '@/pages/auth-return/index';
import TermsAndConditionsPage from '@/pages/terms-and-conditions';
import UserResearchPage from '@/pages/user-research';

import {
  LOGIN_PATH,
  LOGIN_BIOMETRIC_ERROR_PATH,
  PRE_REGISTRATION_INFORMATION_PATH,
  AUTH_RETURN_PATH,
  TERMSANDCONDITIONS_PATH,
  BEGINLOGIN_PATH,
  USER_RESEARCH_PATH,
} from '@/router/paths';
import {
  LOGIN_NAME,
  LOGIN_BIOMETRIC_ERROR_NAME,
  PRE_REGISTRATION_INFORMATION_NAME,
  AUTH_RETURN_NAME,
  TERMSANDCONDITIONS_NAME,
  BEGINLOGIN_NAME,
  USER_RESEARCH_NAME,
} from '@/router/names';

import breadcrumbs from '@/breadcrumbs/login';

import viewedPreRegInstructions from '@/middleware/viewedPreRegInstructions';
import urlResolution from '@/middleware/urlResolution';
import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import { baseNhsAppHelpUrl, appLoginHelpUrl } from '@/router/externalLinks';
import proofLevel from '@/lib/proofLevel';

const getTermsConditionTitle = (store, i18n) => (
  i18n.t(store.state.termsAndConditions.updatedConsentRequired
    ? 'updatedTermsAndConditions.title'
    : 'pageHeaders.termsAndConditions')
);

export const LOGIN = {
  path: LOGIN_PATH,
  name: LOGIN_NAME,
  component: LoginPage,
  meta: {
    isAnonymous: true,
    crumb: breadcrumbs.LOGIN_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    shouldShowContentHeader: false,
    helpUrl: appLoginHelpUrl,
    middleware: [urlResolution],
  },
};

export const LOGIN_BIOMETRIC_ERROR = {
  path: LOGIN_BIOMETRIC_ERROR_PATH,
  name: LOGIN_BIOMETRIC_ERROR_NAME,
  component: LoginBiometricErrorPage,
  meta: {
    headerKey: 'pageTitles.loginBiometricError',
    titleKey: 'pageTitles.loginBiometricError',
    isAnonymous: true,
    crumb: breadcrumbs.LOGIN_BIOMETRIC_ERROR_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpUrl: appLoginHelpUrl,
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
    headerKey: 'pageHeaders.preRegistrationInformation',
    titleKey: 'pageTitles.preRegistrationInformation',
    isAnonymous: true,
    middleware: [urlResolution, viewedPreRegInstructions],
    crumb: breadcrumbs.PRE_REGISTRATION_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpUrl: appLoginHelpUrl,
  },
};

export const BEGIN_LOGIN = {
  path: BEGINLOGIN_PATH,
  name: BEGINLOGIN_NAME,
  meta: {
    isAnonymous: true,
    crumb: breadcrumbs.BEGINLOGIN_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    middleware: [urlResolution],
    helpUrl: appLoginHelpUrl,
  },
};

export const AUTH_RETURN = {
  path: AUTH_RETURN_PATH,
  name: AUTH_RETURN_NAME,
  component: AuthReturnPage,
  meta: {
    isAnonymous: true,
    crumb: breadcrumbs.AUTH_RETURN_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpUrl: appLoginHelpUrl,
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
    crumb: breadcrumbs.TERMSANDCONDITIONS_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpUrl: baseNhsAppHelpUrl,
  },
};

export const USER_RESEARCH = {
  path: USER_RESEARCH_PATH,
  name: USER_RESEARCH_NAME,
  component: UserResearchPage,
  meta: {
    headerKey: 'pageHeaders.userResearch',
    titleKey: 'pageTitles.userResearch',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.USERRESEARCH_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpUrl: baseNhsAppHelpUrl,
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
];
