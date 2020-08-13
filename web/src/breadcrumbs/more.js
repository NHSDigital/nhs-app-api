import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { MORE_NAME } from '@/router/names';

const UPLIFT_MORE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  nativeDisabled: true,
};

export const MORE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  i18nKey: 'more',
  name: MORE_NAME,
  nativeDisabled: true,
};

const DATA_SHARING_OVERVIEW_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  nativeDisabled: false,
};

const DATA_SHARING_WHERE_USED_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  nativeDisabled: false,
};

const DATA_SHARING_DOES_NOT_APPLY_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
  nativeDisabled: false,
};

const DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MORE_CRUMB],
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
