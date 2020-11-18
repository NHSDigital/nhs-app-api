import MorePage from '@/pages/more';
import UpliftMorePage from '@/pages/uplift/more';

import breadcrumbs from '@/breadcrumbs/more';
import {
  UPLIFT_MORE_PATH,
  MORE_PATH,
} from '@/router/paths';
import {
  MORE_NAME,
  UPLIFT_MORE_NAME,
} from '@/router/names';
import { LINKED_PROFILES_SHUTTER_MORE } from '@/router/routes/linked-profiles';

import {
  MESSAGES_MENU_ITEM,
} from '@/middleware/nativeNavigation';
import urlResolution from '@/middleware/urlResolution';

import proofLevel from '@/lib/proofLevel';
import { baseNhsAppHelpUrl } from '@/router/externalLinks';

export const UPLIFT_MORE = {
  path: UPLIFT_MORE_PATH,
  name: UPLIFT_MORE_NAME,
  component: UpliftMorePage,
  meta: {
    headerKey: 'navigation.pages.headers.more',
    titleKey: 'navigation.pages.titles.more',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.UPLIFT_MORE_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: MESSAGES_MENU_ITEM,
    middleware: [urlResolution],
  },
};

export const MORE = {
  path: MORE_PATH,
  name: MORE_NAME,
  component: MorePage,
  meta: {
    headerKey: 'navigation.pages.headers.more',
    titleKey: 'navigation.pages.titles.more',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.MORE_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_MORE,
    }],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export default [
  UPLIFT_MORE,
  MORE,
];
