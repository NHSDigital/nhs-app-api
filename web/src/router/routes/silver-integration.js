import UpliftSilverIntegrationPage from '@/pages/uplift/silver-integration';
import SilverIntegrationFeatureNotAvailablePage from '@/pages/silver-integration/feature-not-available';
import proofLevel from '@/lib/proofLevel';

import {
  UPLIFT_SILVER_INTEGRATION_PATH,
  SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
} from '@/router/paths';
import {
  UPLIFT_SILVER_INTEGRATION_NAME,
  SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_NAME,
} from '@/router/names';

import breadcrumbs from '@/breadcrumbs/silver-integration';

export const UPLIFT_SILVER_INTEGRATION = {
  path: UPLIFT_SILVER_INTEGRATION_PATH,
  name: UPLIFT_SILVER_INTEGRATION_NAME,
  component: UpliftSilverIntegrationPage,
  meta: {
    headerKey: '',
    titleKey: '',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.UPLIFT_SILVER_INTEGRATION_CRUMB,
  },
};

export const ERROR = {
  name: SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_NAME,
  path: SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
  component: SilverIntegrationFeatureNotAvailablePage,
  meta: {
    headerKey: '',
    titleKey: '',
    crumb: breadcrumbs.FEATURE_NOT_AVAILABLE_CRUMB,
    proofLevel: proofLevel.P5,
  },
};

export default [
  UPLIFT_SILVER_INTEGRATION,
  ERROR,
];
