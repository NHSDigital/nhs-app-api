import { accountLinks, footerLinks } from '@/lib/common-links';

describe('common-links', () => {
  it('will verify that footer links are subset of account links', () => {
    const fLinks = footerLinks();
    const aLinks = accountLinks();
    expect(aLinks.length).toBeGreaterThanOrEqual(fLinks.length);
    expect(fLinks.length).toBeGreaterThan(0);
    expect(fLinks.every(link => aLinks.indexOf(link)) !== -1).toBeTruthy();
  });
});
