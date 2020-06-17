import MorePage from '@/pages/more';
import UpliftMorePage from '@/pages/uplift/more';
import DatasharingOverviewPage from '@/pages/data-sharing';
import DatasharingWhereUsedPage from '@/pages/data-sharing/where-used';
import DatasharingDoesNotApplyPage from '@/pages/data-sharing/does-not-apply';
import DatasharingMakeYourChoicePage from '@/pages/data-sharing/make-your-choice';

import breadcrumbs from '@/breadcrumbs/more';
import {
  UPLIFT_MORE_PATH,
  MORE_PATH,
  DATA_SHARING_OVERVIEW_PATH,
  DATA_SHARING_WHERE_USED_PATH,
  DATA_SHARING_DOES_NOT_APPLY_PATH,
  DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
} from '@/router/paths';
import {
  MORE_NAME,
  UPLIFT_MORE_NAME,
  DATA_SHARING_OVERVIEW_NAME,
  DATA_SHARING_WHERE_USED_NAME,
  DATA_SHARING_DOES_NOT_APPLY_NAME,
  DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
} from '@/router/names';
import { LINKED_PROFILES_SHUTTER_MORE } from '@/router/routes/linked-profiles';
import { INDEX } from '@/router/routes/general';

import { MORE_MENU_ITEM } from '@/middleware/nativeNavigation';
import urlResolution from '@/middleware/urlResolution';

import proofLevel from '@/lib/proofLevel';
import { baseNhsAppHelpUrl, ndopHelpUrl } from '@/router/externalLinks';

export const UPLIFT_MORE = {
  path: UPLIFT_MORE_PATH,
  name: UPLIFT_MORE_NAME,
  component: UpliftMorePage,
  meta: {
    headerKey: 'pageHeaders.more',
    titleKey: 'pageTitles.more',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.UPLIFT_MORE_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: MORE_MENU_ITEM,
    middleware: [urlResolution],
  },
};

export const MORE = {
  path: MORE_PATH,
  name: MORE_NAME,
  component: MorePage,
  meta: {
    headerKey: 'pageHeaders.more',
    titleKey: 'pageTitles.more',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.MORE_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_MORE,
    }],
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const DATA_SHARING_OVERVIEW = {
  path: DATA_SHARING_OVERVIEW_PATH,
  name: DATA_SHARING_OVERVIEW_NAME,
  component: DatasharingOverviewPage,
  meta: {
    headerKey: 'pageHeaders.dataSharingOverview',
    titleKey: 'pageTitles.dataSharingOverview',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.DATA_SHARING_OVERVIEW_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const DATA_SHARING_WHERE_USED = {
  path: DATA_SHARING_WHERE_USED_PATH,
  name: DATA_SHARING_WHERE_USED_NAME,
  component: DatasharingWhereUsedPage,
  meta: {
    headerKey: 'pageHeaders.dataSharingWhereUsed',
    titleKey: 'pageTitles.dataSharingWhereUsed',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.DATA_SHARING_WHERE_USED_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const DATA_SHARING_DOES_NOT_APPLY = {
  path: DATA_SHARING_DOES_NOT_APPLY_PATH,
  name: DATA_SHARING_DOES_NOT_APPLY_NAME,
  component: DatasharingDoesNotApplyPage,
  meta: {
    headerKey: 'pageHeaders.dataSharingDoesNotApply',
    titleKey: 'pageTitles.dataSharingDoesNotApply',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.DATA_SHARING_DOES_NOT_APPLY_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: MORE_MENU_ITEM,
  },
};

export const DATA_SHARING_MAKE_YOUR_CHOICE = {
  path: DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
  name: DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
  component: DatasharingMakeYourChoicePage,
  meta: {
    headerKey: 'pageHeaders.dataSharingMakeYourChoice',
    titleKey: 'pageTitles.dataSharingMakeYourChoice',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_MORE,
    crumb: breadcrumbs.DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: MORE_MENU_ITEM,
  },
};


export default [
  UPLIFT_MORE,
  MORE,
  DATA_SHARING_OVERVIEW,
  DATA_SHARING_WHERE_USED,
  DATA_SHARING_DOES_NOT_APPLY,
  DATA_SHARING_MAKE_YOUR_CHOICE,
];
