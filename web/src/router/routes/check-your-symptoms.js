import CheckYourSymptomsPage from '@/pages/check-your-symptoms';
import { CHECKYOURSYMPTOMS_PATH } from '@/router/paths';
import { CHECKYOURSYMPTOMS_NAME } from '@/router/names';
import { baseNhsAppHelpUrl } from '@/router/externalLinks';
import { SYMPTOMS_MENU_ITEM } from '@/middleware/nativeNavigation';
import breadcrumbs from '@/breadcrumbs/symptoms';

export const CHECKYOURSYMPTOMS = {
  path: CHECKYOURSYMPTOMS_PATH,
  name: CHECKYOURSYMPTOMS_NAME,
  component: CheckYourSymptomsPage,
  meta: {
    headerKey: 'navigation.pages.headers.symptoms',
    titleKey: 'navigation.pages.titles.symptoms',
    crumb: breadcrumbs.CHECKYOURSYMPTOMS_CRUMB,
    isAnonymous: true,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: SYMPTOMS_MENU_ITEM,
  },
};

export default [
  CHECKYOURSYMPTOMS,
];
