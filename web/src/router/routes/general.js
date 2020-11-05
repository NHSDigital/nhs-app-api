import IndexPage from '@/pages';
import RedirectorPage from '@/pages/redirector/index';
import NotFoundPage from '@/pages/not-found';
import breadcrumbs from '@/breadcrumbs/general';
import {
  INDEX_PATH,
  INTERSTITIAL_REDIRECTOR_PATH,
  NOT_FOUND_PATH,
} from '@/router/paths';
import {
  INDEX_NAME,
  INTERSTITIAL_REDIRECTOR_NAME,
  NOT_FOUND_NAME,
} from '@/router/names';

import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import urlResolution from '@/middleware/urlResolution';
import proofLevel from '@/lib/proofLevel';
import { baseNhsAppHelpUrl, thirdPartyHelpUrl } from '@/router/externalLinks';

export const INDEX = {
  path: INDEX_PATH,
  name: INDEX_NAME,
  component: IndexPage,
  meta: {
    proofLevel: proofLevel.P5,
    headerKey: 'navigation.pages.headers.home',
    titleKey: 'navigation.pages.titles.home',
    crumb: breadcrumbs.INDEX_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    middleware: [urlResolution],
  },
};

export const REDIRECTOR = {
  path: INTERSTITIAL_REDIRECTOR_PATH,
  name: INTERSTITIAL_REDIRECTOR_NAME,
  component: RedirectorPage,
  meta: {
    headerKey: '',
    titleKey: '',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.REDIRECTOR_CRUMB,
    helpUrl: thirdPartyHelpUrl,
  },
};

export const NOT_FOUND = {
  path: NOT_FOUND_PATH,
  name: NOT_FOUND_NAME,
  component: NotFoundPage,
  meta: {
    headerKey: 'generic.errors.pageNotFound',
    titleKey: 'generic.errors.pageNotFound',
    crumb: {},
    helpUrl: baseNhsAppHelpUrl,
  },
};

export default [
  INDEX,
  REDIRECTOR,
  NOT_FOUND, // this MUST be last otherwise it will match too soon
];
