import { accountLinks, footerLinks } from '@/lib/common-links';

describe('common-links', () => {
  let $env;
  const createEnv = () => ({
    TERMS_AND_CONDITIONS_URL: 'https://terms-and-conditions',
    COOKIES_POLICY_URL: 'https://cookies-policy',
    OPEN_SOURCE_LICENCES_URL: 'https://open-source_lincences',
    PRIVACY_POLICY_URL: 'https://privacy-policy',
    HELP_AND_SUPPORT_URL: 'https://help-and-support',
    ACCESSIBILITY_STATEMENT: 'https://accessibility-statement',
  });
  beforeEach(() => {
    $env = createEnv();
  });
  it('will verify that footer links are subset of account links', () => {
    const fLinks = footerLinks($env);
    const aLinks = accountLinks($env);
    expect(aLinks.length).toBeGreaterThanOrEqual(fLinks.length);
    expect(fLinks.length).toBeGreaterThan(0);
    expect(fLinks.every(link => aLinks.indexOf(link)) !== -1).toBeTruthy();
  });
});
