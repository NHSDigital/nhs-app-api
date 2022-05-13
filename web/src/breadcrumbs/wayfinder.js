import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { WAYFINDER_HELP_NAME, WAYFINDER_NAME } from '@/router/names';
import { APPOINTMENTS_CRUMB } from '@/breadcrumbs/appointments';

const APPOINTMENTS_WAYFINDER_CRUMB = {
  name: WAYFINDER_NAME,
  i18nKey: 'wayfinderReferralsAndAppointments',
  defaultCrumb: [INDEX_CRUMB, APPOINTMENTS_CRUMB],
};

const WAYFINDER_HELP_CRUMB = {
  name: WAYFINDER_HELP_NAME,
  defaultCrumb: [APPOINTMENTS_CRUMB, APPOINTMENTS_WAYFINDER_CRUMB],
};

export default {
  WAYFINDER_HELP_CRUMB,
  APPOINTMENTS_WAYFINDER_CRUMB,
};
