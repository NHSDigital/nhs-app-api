import { INDEX_NAME } from '@/router/names';

export const INDEX_CRUMB = {
  i18nKey: 'home',
  name: INDEX_NAME,
};

const REDIRECTOR_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
};

export default {
  INDEX_CRUMB,
  REDIRECTOR_CRUMB,
};
