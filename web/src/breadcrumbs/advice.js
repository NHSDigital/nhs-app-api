import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { APPOINTMENTS_CRUMB } from '@/breadcrumbs/appointments';
import { ADVICE_NAME } from '@/router/names';

const ADVICE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  i18nKey: 'advice',
  name: ADVICE_NAME,
  nativeDisabled: true,
};

const GET_HEALTH_ADVICE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
};

const GP_ADVICE_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, ADVICE_CRUMB],
  appointmentsCrumb: [INDEX_CRUMB, APPOINTMENTS_CRUMB],
};

export default {
  ADVICE_CRUMB,
  GET_HEALTH_ADVICE_CRUMB,
  GP_ADVICE_CRUMB,
};
