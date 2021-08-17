import each from 'jest-each';
import { footerLinks } from '@/lib/common-links';

import {
  ACCESSIBILITY_STATEMENT_URL,
  PRIVACY_POLICY_URL,
  TERMS_AND_CONDITIONS_URL,
} from '@/router/externalLinks';

const BASE_NHS_APP_HELP_URL = 'http://stubs.local.bitraft.io/help';

describe('common-links', () => {
  each([
    [{ url: TERMS_AND_CONDITIONS_URL, localeLabel: 'more.termsOfUse' }],
    [{ url: PRIVACY_POLICY_URL, localeLabel: 'more.privacyPolicy' }],
    [{ url: BASE_NHS_APP_HELP_URL, localeLabel: 'more.helpAndSupport' }],
    [{ url: ACCESSIBILITY_STATEMENT_URL, localeLabel: 'more.accessibilityStatement' }],
  ]).it('will verify that footer links contain %s', (link) => {
    const $env = { BASE_NHS_APP_HELP_URL };
    const fLinks = footerLinks($env);
    expect(fLinks.length).toBeGreaterThan(0);
    expect(fLinks).toContainEqual(link);
  });
});
