import LogoutPage from '@/pages/logout';

import { LOGOUT_PATH } from '@/router/paths';
import { LOGOUT_NAME } from '@/router/names';

import { appLoginHelpUrl } from '@/router/externalLinks';
import proofLevel from '@/lib/proofLevel';
import breadcrumbs from '@/breadcrumbs/logout';

export const LOGOUT = {
  path: LOGOUT_PATH,
  name: LOGOUT_NAME,
  component: LogoutPage,
  meta: {
    headerKey: 'pageHeaders.logout',
    titleKey: 'pageTitles.logout',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.LOGOUT_CRUMB,
    helpUrl: appLoginHelpUrl,
  },
};

export default [
  LOGOUT,
];
