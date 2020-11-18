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

export default {
  UPLIFT_MORE_CRUMB,
  MORE_CRUMB,
};
