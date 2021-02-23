import MoreIndexPage from '@/pages/more';
import MoreCookiesPage from '@/pages/more/cookies';
import MoreNotificationsPage from '@/pages/more/notifications';
import MoreLoginSettingsIndexPage from '@/pages/more/login-settings';
import MoreLoginSettingsErrorPage from '@/pages/more/login-settings/error';

import {
  MORE_PATH,
  MORE_COOKIES_PATH,
  MORE_NOTIFICATIONS_PATH,
  MORE_LOGIN_SETTINGS_PATH,
  MORE_LOGIN_SETTINGS_ERROR_PATH,
  INDEX_PATH,
} from '@/router/paths';
import {
  MORE_NAME,
  MORE_COOKIES_NAME,
  MORE_NOTIFICATIONS_NAME,
  MORE_LOGIN_SETTINGS_NAME,
  MORE_LOGIN_SETTINGS_ERROR_NAME,
  INDEX_NAME,
} from '@/router/names';
import { LINKED_PROFILES_SHUTTER_MORE } from '@/router/routes/linked-profiles';

import { moreHelpUrl, appLoginHelpUrl, accountSecurityHelpUrl } from '@/router/externalLinks';
import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import proofLevel from '@/lib/proofLevel';
import breadcrumbs from '@/breadcrumbs/more';
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

export const MORE = {
  path: MORE_PATH,
  name: MORE_NAME,
  component: MoreIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.more',
    titleKey: 'navigation.pages.titles.more',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_CRUMB,
    helpUrl: moreHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_MORE,
    }],
  },
};

export const MORE_COOKIES = {
  path: MORE_COOKIES_PATH,
  name: MORE_COOKIES_NAME,
  component: MoreCookiesPage,
  meta: {
    headerKey: 'navigation.pages.headers.cookies',
    titleKey: 'navigation.pages.titles.cookies',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_COOKIES_CRUMB,
    helpUrl: accountSecurityHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
  },
};

export const MORE_NOTIFICATIONS = {
  path: MORE_NOTIFICATIONS_PATH,
  name: MORE_NOTIFICATIONS_NAME,
  component: MoreNotificationsPage,
  meta: {
    headerKey: 'navigation.pages.headers.notifications',
    titleKey: 'navigation.pages.titles.notifications',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_COOKIES_CRUMB,
    helpUrl: moreHelpUrl,
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

export const MORE_LOGIN_SETTINGS_INDEX = {
  path: MORE_LOGIN_SETTINGS_PATH,
  name: MORE_LOGIN_SETTINGS_NAME,
  component: MoreLoginSettingsIndexPage,
  meta: {
    headerKey: (store, i18n) => getLoginSettingsHeaderTitle('navigation.pages.headers', store, i18n),
    titleKey: (store, i18n) => getLoginSettingsHeaderTitle('navigation.pages.titles', store, i18n),
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_LOGIN_SETTINGS_INDEX_CRUMB,
    helpUrl: appLoginHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: MORE,
    }],
  },
};

export const MORE_LOGIN_SETTINGS_ERROR = {
  path: MORE_LOGIN_SETTINGS_ERROR_PATH,
  name: MORE_LOGIN_SETTINGS_ERROR_NAME,
  component: MoreLoginSettingsErrorPage,
  meta: {
    headerKey: (store, i18n) => getLoginSettingsErrorHeaderTitle('navigation.pages.headers', store, i18n),
    titleKey: (store, i18n) => getLoginSettingsErrorHeaderTitle('navigation.pages.titles', store, i18n),
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MORE_LOGIN_SETTINGS_ERROR_CRUMB,
    helpUrl: appLoginHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: MORE,
    }],
  },
};

export default [
  MORE,
  MORE_COOKIES,
  MORE_NOTIFICATIONS,
  MORE_LOGIN_SETTINGS_INDEX,
  MORE_LOGIN_SETTINGS_ERROR,
];
