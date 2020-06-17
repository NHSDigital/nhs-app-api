import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { CHECKYOURSYMPTOMS_NAME, SYMPTOMS_NAME } from '@/router/names';

const SYMPTOMS_CRUMB = {
  i18nKey: 'symptoms',
  defaultCrumb: [INDEX_CRUMB],
  name: SYMPTOMS_NAME,
  nativeDisabled: true,
};

const CHECKYOURSYMPTOMS_CRUMB = {
  i18nKey: 'symptoms',
  defaultCrumb: [INDEX_CRUMB],
  name: CHECKYOURSYMPTOMS_NAME,
};

export default {
  SYMPTOMS_CRUMB,
  CHECKYOURSYMPTOMS_CRUMB,
};
