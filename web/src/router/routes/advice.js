import AdviceIndexPage from '@/pages/advice';
import { ADVICE_PATH, SYMPTOMS_PATH } from '@/router/paths';
import { ADVICE_NAME, SYMPTOMS_NAME } from '@/router/names';
import { LINKED_PROFILES_SHUTTER_ADVICE } from '@/router/routes/linked-profiles';
import { baseNhsAppHelpUrl } from '@/router/externalLinks';
import { ADVICE_MENU_ITEM } from '@/middleware/nativeNavigation';
import proofLevel from '@/lib/proofLevel';
import breadcrumbs from '@/breadcrumbs/advice';

export const ADVICE = {
  path: ADVICE_PATH,
  name: ADVICE_NAME,
  component: AdviceIndexPage,
  meta: {
    headerKey: 'navigation.pages.headers.advice',
    titleKey: 'navigation.pages.headers.advice',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.ADVICE_CRUMB,
    helpUrl: baseNhsAppHelpUrl,
    nativeNavigation: ADVICE_MENU_ITEM,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_ADVICE,
    }],
  },
};

export const SYMPTOMS = {
  path: SYMPTOMS_PATH,
  name: SYMPTOMS_NAME,
  redirect: ADVICE.path,
};

export default [
  ADVICE,
  SYMPTOMS,
];
