import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { HEALTH_RECORDS_CRUMB } from '@/breadcrumbs/medicalRecord';

const DATA_SHARING_OVERVIEW_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
  nativeDisabled: false,
};

const DATA_SHARING_WHERE_USED_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
  nativeDisabled: false,
};

const DATA_SHARING_DOES_NOT_APPLY_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
  nativeDisabled: false,
};

const DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
  nativeDisabled: false,
};

export default {
  DATA_SHARING_OVERVIEW_CRUMB,
  DATA_SHARING_WHERE_USED_CRUMB,
  DATA_SHARING_DOES_NOT_APPLY_CRUMB,
  DATA_SHARING_MAKE_YOUR_CHOICE_CRUMB,
};
