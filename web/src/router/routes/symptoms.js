import SymptomsIndexPage from '@/pages/symptoms';
import { SYMPTOMS_PATH } from '@/router/paths';
import { SYMPTOMS_NAME } from '@/router/names';
import { LINKED_PROFILES_SHUTTER_SYMPTOMS } from '@/router/routes/linked-profiles';
import { baseNhsAppHelpUrl } from '@/router/externalLinks';
import { SYMPTOMS_MENU_ITEM } from '@/middleware/nativeNavigation';
import proofLevel from '@/lib/proofLevel';
import breadcrumbs from '@/breadcrumbs/symptoms';

export const SYMPTOMS = {
  path: SYMPTOMS_PATH,
  name: SYMPTOMS_NAME,
  component: SymptomsIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.symptoms',
    titleKey: 'navigation.pages.titles.symptoms',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.SYMPTOMS_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: SYMPTOMS_MENU_ITEM,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_SYMPTOMS,
    }],
  },
};

export default [
  SYMPTOMS,
];
