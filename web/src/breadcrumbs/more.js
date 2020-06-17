import { INDEX_CRUMB } from '@/breadcrumbs/general';
import {
  UPLIFT_MORE_NAME,
  MORE_NAME,
  DATA_SHARING_OVERVIEW_NAME,
  DATA_SHARING_WHERE_USED_NAME,
  DATA_SHARING_DOES_NOT_APPLY_NAME,
  DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
} from '@/router/names';

const UPLIFT_MORE_CRUMB = {
  i18nKey: 'more',
  defaultCrumb: [INDEX_CRUMB],
  name: UPLIFT_MORE_NAME,
  nativeDisabled: true,
};

export const MORE_CRUMB = {
  i18nKey: 'more',
  defaultCrumb: [INDEX_CRUMB],
  name: MORE_NAME,
  nativeDisabled: true,
};

const DATA_SHARING_OVERVIEW_CRUMB = {
  i18nKey: 'dataSharingOverview',
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  name: DATA_SHARING_OVERVIEW_NAME,
  nativeDisabled: false,
};

const DATA_SHARING_WHERE_USED_CRUMB = {
  i18nKey: 'dataSharingWhereUsed',
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  name: DATA_SHARING_WHERE_USED_NAME,
  nativeDisabled: false,
};

const DATA_SHARING_DOES_NOT_APPLY_CRUMB = {
  i18nKey: 'dataSharingDoesNotApply',
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  name: DATA_SHARING_DOES_NOT_APPLY_NAME,
  nativeDisabled: false,
};

const DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB = {
  i18nKey: 'dataSharingMakeYourChoice',
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  name: DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
  nativeDisabled: false,
};

export default {
  UPLIFT_MORE_CRUMB,
  MORE_CRUMB,
  DATA_SHARING_OVERVIEW_CRUMB,
  DATA_SHARING_WHERE_USED_CRUMB,
  DATA_SHARING_DOES_NOT_APPLY_CRUMB,
  DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB,
};
