import GetHealthAdvicePage from '@/pages/get-health-advice';
import { GET_HEALTH_ADVICE_PATH, CHECKYOURSYMPTOMS_PATH } from '@/router/paths';
import { GET_HEALTH_ADVICE_NAME, CHECKYOURSYMPTOMS_NAME } from '@/router/names';
import { baseNhsAppHelpUrl } from '@/router/externalLinks';
import { ADVICE_MENU_ITEM } from '@/middleware/nativeNavigation';
import breadcrumbs from '@/breadcrumbs/advice';

export const GET_HEALTH_ADVICE = {
  path: GET_HEALTH_ADVICE_PATH,
  name: GET_HEALTH_ADVICE_NAME,
  component: GetHealthAdvicePage,
  meta: {
    headerKey: 'navigation.pages.headers.advice',
    titleKey: 'navigation.pages.headers.advice',
    crumb: breadcrumbs.GET_HEALTH_ADVICE_CRUMB,
    isAnonymous: true,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: ADVICE_MENU_ITEM,
  },
};

export const CHECKYOURSYMPTOMS = {
  path: CHECKYOURSYMPTOMS_PATH,
  name: CHECKYOURSYMPTOMS_NAME,
  redirect: GET_HEALTH_ADVICE.path,
};

export default [
  GET_HEALTH_ADVICE,
  CHECKYOURSYMPTOMS,
];
