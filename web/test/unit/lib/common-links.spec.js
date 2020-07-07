import { accountLinks, footerLinks } from '@/lib/common-links';

describe('common-links', () => {
  let $env;
  const createEnv = () => ({
    TERMS_AND_CONDITIONS_URL: 'https://terms',
    COOKIES_POLICY_URL: 'https://cookies',
    OPEN_SOURCE_LICENCES_URL: 'https://open-source',
    PRIVACY_POLICY_URL: 'https://privacy',
    HELP_AND_SUPPORT_URL: 'https://help',
    ACCESSIBILITY_STATEMENT_URL: 'https://accessibility',
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
