import DatasharingOverviewPage from '@/pages/data-sharing';
import DatasharingWhereUsedPage from '@/pages/data-sharing/where-used';
import DatasharingDoesNotApplyPage from '@/pages/data-sharing/does-not-apply';
import DatasharingMakeYourChoicePage from '@/pages/data-sharing/make-your-choice';

import breadcrumbs from '@/breadcrumbs/data-sharing';
import {
  DATA_SHARING_OVERVIEW_PATH,
  DATA_SHARING_WHERE_USED_PATH,
  DATA_SHARING_DOES_NOT_APPLY_PATH,
  DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
} from '@/router/paths';
import {
  DATA_SHARING_OVERVIEW_NAME,
  DATA_SHARING_WHERE_USED_NAME,
  DATA_SHARING_DOES_NOT_APPLY_NAME,
  DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
} from '@/router/names';
import { INDEX } from '@/router/routes/general';
import { UPLIFT_GP_MEDICAL_RECORD } from '@/router/routes/medical-record';

import {
  YOUR_RECORD_MENU_ITEM,
} from '@/middleware/nativeNavigation';

import proofLevel from '@/lib/proofLevel';
import { ndopHelpUrl } from '@/router/externalLinks';

export const DATA_SHARING_OVERVIEW = {
  path: DATA_SHARING_OVERVIEW_PATH,
  name: DATA_SHARING_OVERVIEW_NAME,
  component: DatasharingOverviewPage,
  meta: {
    headerKey: 'navigation.pages.headers.dataSharingOverview',
    titleKey: 'navigation.pages.titles.dataSharingOverview',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DATA_SHARING_OVERVIEW_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
  },
};

export const DATA_SHARING_WHERE_USED = {
  path: DATA_SHARING_WHERE_USED_PATH,
  name: DATA_SHARING_WHERE_USED_NAME,
  component: DatasharingWhereUsedPage,
  meta: {
    headerKey: 'navigation.pages.headers.dataSharingWhereUsed',
    titleKey: 'navigation.pages.titles.dataSharingWhereUsed',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DATA_SHARING_WHERE_USED_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
  },
};

export const DATA_SHARING_DOES_NOT_APPLY = {
  path: DATA_SHARING_DOES_NOT_APPLY_PATH,
  name: DATA_SHARING_DOES_NOT_APPLY_NAME,
  component: DatasharingDoesNotApplyPage,
  meta: {
    headerKey: 'navigation.pages.headers.dataSharingDoesNotApply',
    titleKey: 'navigation.pages.titles.dataSharingDoesNotApply',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DATA_SHARING_DOES_NOT_APPLY_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
  },
};

export const DATA_SHARING_MAKE_YOUR_CHOICE = {
  path: DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
  name: DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
  component: DatasharingMakeYourChoicePage,
  meta: {
    headerKey: 'navigation.pages.headers.dataSharingMakeYourChoice',
    titleKey: 'navigation.pages.titles.dataSharingMakeYourChoice',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB,
    helpUrl: ndopHelpUrl,
    redirectRules: [{
      condition: 'device/isNativeApp',
      value: false,
      route: INDEX,
    }],
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
  },
};


export default [
  DATA_SHARING_OVERVIEW,
  DATA_SHARING_WHERE_USED,
  DATA_SHARING_DOES_NOT_APPLY,
  DATA_SHARING_MAKE_YOUR_CHOICE,
];
