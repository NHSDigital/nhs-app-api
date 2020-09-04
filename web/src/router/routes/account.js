import AccountIndexPage from '@/pages/account';
import AccountCookiesPage from '@/pages/account/cookies';
import AccountNotificationsPage from '@/pages/account/notifications';
import AccountLoginSettingsIndexPage from '@/pages/account/login-settings';
import AccountLoginSettingsErrorPage from '@/pages/account/login-settings/error';

import {
  ACCOUNT_PATH,
  ACCOUNT_COOKIES_PATH,
  ACCOUNT_NOTIFICATIONS_PATH,
  LOGIN_SETTINGS_PATH,
  LOGIN_SETTINGS_ERROR_PATH,
  INDEX_PATH,
} from '@/router/paths';
import {
  ACCOUNT_NAME,
  ACCOUNT_COOKIES_NAME,
  ACCOUNT_NOTIFICATIONS_NAME,
  LOGIN_SETTINGS_NAME,
  LOGIN_SETTINGS_ERROR_NAME,
  INDEX_NAME,
} from '@/router/names';
import { LINKED_PROFILES_SHUTTER_SETTINGS } from '@/router/routes/linked-profiles';

import { accountHelpUrl, appLoginHelpUrl, accountSecurityHelpUrl } from '@/router/externalLinks';
import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import proofLevel from '@/lib/proofLevel';
import breadcrumbs from '@/breadcrumbs/account';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import biometricTypes from '@/lib/biometrics/biometricTypes';
import get from 'lodash/fp/get';
import { INDEX } from '@/router/routes/general';

const getLoginSettingsHeaderTitle = (prefix, store, i18n) => {
  const biometricTypeReference = get('state.loginSettings.biometricLocaleReference')(store);
  return (biometricTypeReference === undefined)
    ? i18n.t(`${prefix}.loginSettingsNoType`)
    : i18n.t(`${prefix}.loginSettings`, { biometricType: i18n.t(biometricTypeReference) });
};

const getLoginSettingsErrorHeaderTitle = (prefix, store, i18n) => {
  const biometricTypeReference = get('state.loginSettings.biometricLocaleReference')(store);
  const errorCode = get('state.loginSettings.errorCode')(store);

  let biometricType = i18n.t(biometricTypeReference);

  if (biometricType.toLocaleLowerCase() === biometricTypes.Fingerprint.toLocaleLowerCase()) {
    biometricType = biometricType.toLocaleLowerCase();
  }

  return errorCode === biometricErrorCodes.CannotFindBiometrics
    ? i18n.t(`${prefix}.loginSettingsErrorCannotFind`, { biometricType })
    : i18n.t(`${prefix}.loginSettingsErrorCannotChange`, { biometricType });
};

export const ACCOUNT = {
  path: ACCOUNT_PATH,
  name: ACCOUNT_NAME,
  component: AccountIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.settings',
    titleKey: 'navigation.pages.titles.settings',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.ACCOUNT_CRUMB,
    helpUrl: accountHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_SETTINGS,
    }],
  },
};

export const ACCOUNT_COOKIES = {
  path: ACCOUNT_COOKIES_PATH,
  name: ACCOUNT_COOKIES_NAME,
  component: AccountCookiesPage,
  meta: {
    headerKey: 'navigation.pages.headers.cookies',
    titleKey: 'navigation.pages.titles.cookies',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.ACCOUNT_COOKIES_CRUMB,
    helpUrl: accountSecurityHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const ACCOUNT_NOTIFICATIONS = {
  path: ACCOUNT_NOTIFICATIONS_PATH,
  name: ACCOUNT_NOTIFICATIONS_NAME,
  component: AccountNotificationsPage,
  meta: {
    headerKey: 'navigation.pages.headers.notifications',
    titleKey: 'navigation.pages.titles.notifications',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.ACCOUNT_COOKIES_CRUMB,
    helpUrl: accountHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }, {
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_SETTINGS,
    }],
    sjrRedirectRules: [{
      journey_disabled: 'notifications',
      url: INDEX_PATH,
      name: INDEX_NAME,
    }],
  },
};

export const ACCOUNT_LOGIN_SETTINGS_INDEX = {
  path: LOGIN_SETTINGS_PATH,
  name: LOGIN_SETTINGS_NAME,
  component: AccountLoginSettingsIndexPage,
  meta: {
    headerKey: (store, i18n) => getLoginSettingsHeaderTitle('navigation.pages.headers', store, i18n),
    titleKey: (store, i18n) => getLoginSettingsHeaderTitle('navigation.pages.titles', store, i18n),
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.ACCOUNT_LOGIN_SETTINGS_INDEX_CRUMB,
    helpUrl: appLoginHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: ACCOUNT,
    }],
  },
};

export const ACCOUNT_LOGIN_SETTINGS_ERROR = {
  path: LOGIN_SETTINGS_ERROR_PATH,
  name: LOGIN_SETTINGS_ERROR_NAME,
  component: AccountLoginSettingsErrorPage,
  meta: {
    headerKey: (store, i18n) => getLoginSettingsErrorHeaderTitle('navigation.pages.headers', store, i18n),
    titleKey: (store, i18n) => getLoginSettingsErrorHeaderTitle('navigation.pages.titles', store, i18n),
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.ACCOUNT_LOGIN_SETTINGS_ERROR_CRUMB,
    helpUrl: appLoginHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: ACCOUNT,
    }],
  },
};

export default [
  ACCOUNT,
  ACCOUNT_COOKIES,
  ACCOUNT_NOTIFICATIONS,
  ACCOUNT_LOGIN_SETTINGS_INDEX,
  ACCOUNT_LOGIN_SETTINGS_ERROR,
];
