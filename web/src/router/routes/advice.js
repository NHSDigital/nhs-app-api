import AdviceIndexPage from '@/pages/advice';
import GpAdvicePage from '@/pages/advice/gp-advice';

import { ADVICE_PATH, GP_ADVICE_PATH } from '@/router/paths';
import { ADVICE_NAME, GP_ADVICE_NAME } from '@/router/names';
import { LINKED_PROFILES_SHUTTER_ADVICE } from '@/router/routes/linked-profiles';
import { UPLIFT_APPOINTMENTS } from '@/router/routes/appointments';
import { baseNhsAppHelpUrl, onlineConsultationsHelpUrl } from '@/router/externalLinks';
import sjrRedirectRules from '@/router/sjrRedirectRules';

import { ADVICE_MENU_ITEM } from '@/middleware/nativeNavigation';

import proofLevel from '@/lib/proofLevel';
import CaptionSize from '@/lib/caption-size';

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

export const GP_ADVICE = {
  path: GP_ADVICE_PATH,
  name: GP_ADVICE_NAME,
  component: GpAdvicePage,
  meta: {
    captionKey: 'navigation.pages.headers.adviceGpAdvice',
    captionSize: CaptionSize.Medium,
    titleKey: 'navigation.pages.titles.adviceGpAdvice',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_APPOINTMENTS,
    crumb: breadcrumbs.GP_ADVICE_CRUMB,
    helpUrl: onlineConsultationsHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.gpAdviceDisabledRedirect],
    warningBanner: true,
  },
};

export default [
  ADVICE,
  GP_ADVICE,
];
