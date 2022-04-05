import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { WAYFINDER_NAME } from '@/router/names';
import { APPOINTMENTS_CRUMB } from '@/breadcrumbs/appointments';

const APPOINTMENTS_WAYFINDER_CRUMB = {
  name: WAYFINDER_NAME,
  defaultCrumb: [INDEX_CRUMB, APPOINTMENTS_CRUMB],
};

export default {
  APPOINTMENTS_WAYFINDER_CRUMB,
};
