import LogoutPage from '@/pages/logout';

import { LOGOUT_PATH } from '@/router/paths';
import { LOGOUT_NAME } from '@/router/names';

import { appLoginHelpUrl } from '@/router/externalLinks';
import proofLevel from '@/lib/proofLevel';

export const LOGOUT = {
  path: LOGOUT_PATH,
  name: LOGOUT_NAME,
  component: LogoutPage,
  meta: {
    headerKey: 'navigation.pages.headers.logout',
    titleKey: 'navigation.pages.titles.logout',
    proofLevel: proofLevel.P5,
    crumb: {},
    helpUrl: appLoginHelpUrl,
  },
};

export default [
  LOGOUT,
];
