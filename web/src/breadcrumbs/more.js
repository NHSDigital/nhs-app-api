import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { MORE_NAME } from '@/router/names';

export const MORE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  i18nKey: 'more',
  name: MORE_NAME,
  nativeDisabled: true,
};

const MORE_COOKIES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
};

const MORE_NOTIFICATIONS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
};

const MORE_LOGIN_SETTINGS_INDEX_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
};

const MORE_LOGIN_SETTINGS_ERROR_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
};

export default {
  MORE_CRUMB,
  MORE_COOKIES_CRUMB,
  MORE_NOTIFICATIONS_CRUMB,
  MORE_LOGIN_SETTINGS_INDEX_CRUMB,
  MORE_LOGIN_SETTINGS_ERROR_CRUMB,
};
