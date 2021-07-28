import each from 'jest-each';
import { footerLinks } from '@/lib/common-links';

import {
  ACCESSIBILITY_STATEMENT_URL,
  HELP_AND_SUPPORT_URL,
  PRIVACY_POLICY_URL,
  TERMS_AND_CONDITIONS_URL,
} from '@/router/externalLinks';

describe('common-links', () => {
  each([
    [{ url: TERMS_AND_CONDITIONS_URL, localeLabel: 'more.termsOfUse' }],
    [{ url: PRIVACY_POLICY_URL, localeLabel: 'more.privacyPolicy' }],
    [{ url: HELP_AND_SUPPORT_URL, localeLabel: 'more.helpAndSupport' }],
    [{ url: ACCESSIBILITY_STATEMENT_URL, localeLabel: 'more.accessibilityStatement' }],
  ]).it('will verify that footer links contain %s', (link) => {
    const fLinks = footerLinks();
    expect(fLinks.length).toBeGreaterThan(0);
    expect(fLinks).toContainEqual(link);
  });
});
