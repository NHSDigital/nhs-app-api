import { INDEX_CRUMB } from '@/breadcrumbs/general';
import {
  ACCOUNT_NAME,
  ACCOUNT_COOKIES_NAME,
  ACCOUNT_NOTIFICATIONS_NAME,
  LOGIN_SETTINGS_NAME,
  LOGIN_SETTINGS_ERROR_NAME,
} from '@/router/names';

export const ACCOUNT_CRUMB = {
  i18nKey: 'account',
  defaultCrumb: [INDEX_CRUMB],
  name: ACCOUNT_NAME,
  nativeDisabled: true,
};

const ACCOUNT_COOKIES_CRUMB = {
  i18nKey: 'accountCookies',
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
  name: ACCOUNT_COOKIES_NAME,
  nativeDisabled: false,
};

const ACCOUNT_NOTIFICATIONS_CRUMB = {
  i18nKey: 'accountNotifications',
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
  name: ACCOUNT_NOTIFICATIONS_NAME,
  nativeDisabled: false,
};

const ACCOUNT_LOGIN_SETTINGS_INDEX_CRUMB = {
  i18nKey: 'loginSettings',
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
  name: LOGIN_SETTINGS_NAME,
  nativeDisabled: false,
};

const ACCOUNT_LOGIN_SETTINGS_ERROR_CRUMB = {
  i18nKey: 'loginSettingsError',
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB, ACCOUNT_LOGIN_SETTINGS_INDEX_CRUMB],
  name: LOGIN_SETTINGS_ERROR_NAME,
  nativeDisabled: false,
};

export default {
  ACCOUNT_CRUMB,
  ACCOUNT_COOKIES_CRUMB,
  ACCOUNT_NOTIFICATIONS_CRUMB,
  ACCOUNT_LOGIN_SETTINGS_INDEX_CRUMB,
  ACCOUNT_LOGIN_SETTINGS_ERROR_CRUMB,
};
