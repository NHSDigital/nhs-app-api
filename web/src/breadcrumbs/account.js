import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { ACCOUNT_NAME } from '@/router/names';

export const ACCOUNT_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  i18nKey: 'account',
  name: ACCOUNT_NAME,
  nativeDisabled: true,
};

const ACCOUNT_COOKIES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
};

const ACCOUNT_NOTIFICATIONS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
};

const ACCOUNT_LOGIN_SETTINGS_INDEX_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
};

const ACCOUNT_LOGIN_SETTINGS_ERROR_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, ACCOUNT_CRUMB],
};

export default {
  ACCOUNT_CRUMB,
  ACCOUNT_COOKIES_CRUMB,
  ACCOUNT_NOTIFICATIONS_CRUMB,
  ACCOUNT_LOGIN_SETTINGS_INDEX_CRUMB,
  ACCOUNT_LOGIN_SETTINGS_ERROR_CRUMB,
};
