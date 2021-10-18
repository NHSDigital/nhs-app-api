import MoreIndexPage from '@/pages/more';
import MoreAccountAndSettingsIndexPage from '@/pages/more/account-and-settings';
import MoreAccountAndSettingsLoginSettingsIndexPage from '@/pages/more/account-and-settings/login-settings';
import MoreAccountAndSettingsLoginSettingsErrorPage from '@/pages/more/account-and-settings/login-settings/error';
import MoreAccountAndSettingsManageNotificationsPage from '@/pages/more/account-and-settings/manage-notifications';
import MoreAccountAndSettingsLegalAndCookiesIndexPage from '@/pages/more/account-and-settings/legal-and-cookies';
import MoreAccountAndSettingsLegalAndCookiesMangeCookiesPage from '@/pages/more/account-and-settings/legal-and-cookies/manage-cookies';

import {
  MORE_PATH,
  INDEX_PATH,
  MORE_ACCOUNTANDSETTINGS_PATH,
  MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_PATH,
  MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_MANAGECOOKIES_PATH,
  MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_ERROR_PATH,
  MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH,
} from '@/router/paths';
import {
  MORE_NAME,
  INDEX_NAME,
  MORE_ACCOUNTANDSETTINGS_NAME,
  MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_NAME,
  MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_ERROR_NAME,
  MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_NAME,
  MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_MANAGECOOKIES_NAME,
  MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_NAME,
} from '@/router/names';
import { LINKED_PROFILES_SHUTTER_MORE } from '@/router/routes/linked-profiles';
import {
  APP_LOGIN_HELP_PATH,
  SECURITY_AND_PRIVACY_HELP_PATH,
  MANAGING_YOUR_NHS_APP_ACCOUNT_HELP_PATH,
  NHS_APP_ACCOUNT_AND_SETTINGS_HELP_PATH,
} from '@/router/externalLinks';
import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import proofLevel from '@/lib/proofLevel';
import breadcrumbs from '@/breadcrumbs/more';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import biometricTypes from '@/lib/biometrics/biometricTypes';
import { INDEX } from '@/router/routes/general';

const getLoginSettingsHeaderTitle = (prefix, store, i18n) => {
  const biometricType = store.getters['loginSettings/biometricType'];
  if (biometricType === biometricTypes.None) {
    return i18n.t(`${prefix}.loginSettingsNoType`);
  }
  return i18n.t(
    `${prefix}.loginSettings`,
    { biometricType: i18n.t(`loginSettings.biometrics.biometricType.${biometricType}`) },
  );
};

const getLoginSettingsErrorHeaderTitle = (prefix, store, i18n) => {
  const biometricType = store.getters['loginSettings/biometricType'];
  const biometricTypeName = i18n.t(`loginSettings.biometrics.errors.title.${biometricType}`);

  const errorCode = store.getters['loginSettings/biometricError'];

  return errorCode === biometricErrorCodes.CannotFindBiometrics
    ? i18n.t(`${prefix}.loginSettingsErrorCannotFind`, { biometricType: biometricTypeName })
    : i18n.t(`${prefix}.loginSettingsErrorCannotChange`, { biometricType: biometricTypeName });
};

export const MORE = {
  path: MORE_PATH,
  name: MORE_NAME,
  component: MoreIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.more',
    titleKey: 'navigation.pages.titles.more',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_MORE,
    }],
  },
};

export const MORE_ACCOUNT_AND_SETTINGS_INDEX = {
  path: MORE_ACCOUNTANDSETTINGS_PATH,
  name: MORE_ACCOUNTANDSETTINGS_NAME,
  component: MoreAccountAndSettingsIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.accountAndSettings',
    titleKey: 'navigation.pages.titles.accountAndSettings',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_ACCOUNT_AND_SETTINGS_INDEX_CRUMB,
    helpPath: NHS_APP_ACCOUNT_AND_SETTINGS_HELP_PATH,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_INDEX = {
  path: MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_PATH,
  name: MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_NAME,
  component: MoreAccountAndSettingsLegalAndCookiesIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.legalAndCookies',
    titleKey: 'navigation.pages.titles.legalAndCookies',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_INDEX_CRUMB,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_MANAGE_COOKIES = {
  path: MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_MANAGECOOKIES_PATH,
  name: MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_MANAGECOOKIES_NAME,
  component: MoreAccountAndSettingsLegalAndCookiesMangeCookiesPage,
  meta: {
    headerKey: 'navigation.pages.headers.cookies',
    titleKey: 'navigation.pages.titles.cookies',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_MANAGE_COOKIES_CRUMB,
    helpPath: SECURITY_AND_PRIVACY_HELP_PATH,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const MORE_ACCOUNT_AND_SETTINGS_MANAGE_NOTIFICATIONS = {
  path: MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
  name: MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_NAME,
  component: MoreAccountAndSettingsManageNotificationsPage,
  meta: {
    headerKey: 'navigation.pages.headers.manageNotifications',
    titleKey: 'navigation.pages.titles.manageNotifications',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_INDEX_CRUMB,
    helpPath: MANAGING_YOUR_NHS_APP_ACCOUNT_HELP_PATH,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }, {
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_MORE,
    }],
    sjrRedirectRules: [{
      journey_disabled: 'notifications',
      url: INDEX_PATH,
      name: INDEX_NAME,
    }],
  },
};

export const MORE_ACCOUNT_AND_SETTINGS_LOGIN_SETTINGS_INDEX = {
  path: MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_PATH,
  name: MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_NAME,
  component: MoreAccountAndSettingsLoginSettingsIndexPage,
  meta: {
    headerKey: (store, i18n) => getLoginSettingsHeaderTitle('navigation.pages.headers', store, i18n),
    titleKey: (store, i18n) => getLoginSettingsHeaderTitle('navigation.pages.titles', store, i18n),
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_LOGIN_SETTINGS_INDEX_CRUMB,
    helpPath: APP_LOGIN_HELP_PATH,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: MORE,
    }],
  },
  alias: [MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH, MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH,
    MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH, MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH],
};

export const MORE_ACCOUNT_AND_SETTINGS_LOGIN_SETTINGS_ERROR = {
  path: MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_ERROR_PATH,
  name: MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_ERROR_NAME,
  component: MoreAccountAndSettingsLoginSettingsErrorPage,
  meta: {
    headerKey: (store, i18n) => getLoginSettingsErrorHeaderTitle('navigation.pages.headers', store, i18n),
    titleKey: (store, i18n) => getLoginSettingsErrorHeaderTitle('navigation.pages.titles', store, i18n),
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_LOGIN_SETTINGS_ERROR_CRUMB,
    helpPath: APP_LOGIN_HELP_PATH,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: MORE,
    }],
  },
};

export default [
  MORE,
  MORE_ACCOUNT_AND_SETTINGS_INDEX,
  MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_INDEX,
  MORE_ACCOUNT_AND_SETTINGS_LEGAL_AND_COOKIES_MANAGE_COOKIES,
  MORE_ACCOUNT_AND_SETTINGS_MANAGE_NOTIFICATIONS,
  MORE_ACCOUNT_AND_SETTINGS_LOGIN_SETTINGS_INDEX,
  MORE_ACCOUNT_AND_SETTINGS_LOGIN_SETTINGS_ERROR,
];
