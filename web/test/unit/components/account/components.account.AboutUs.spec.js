import AboutUs from '@/components/account/AboutUs';
import { createStore, mount } from '../../helpers';

describe('AboutUs', () => {
  let wrapper;
  let $store;
  const URL_EXTERNAL = 'www.foo.com';

  const mountAboutUs = () => mount(AboutUs, { $store });

  beforeEach(() => {
    wrapper = mountAboutUs();
    $store = createStore({ $env: {
      TERMS_AND_CONDITIONS_URL: URL_EXTERNAL,
      PRIVACY_POLICY_URL: URL_EXTERNAL,
      OPEN_SOURCE_LICENCES_URL: URL_EXTERNAL,
      HELP_AND_SUPPORT_URL: URL_EXTERNAL,
      ACCESSIBILITY_STATEMENT: URL_EXTERNAL } });
  });

  it('will have a header', () => {
    const header = wrapper.find('h2');
    expect(header.exists()).toBe(true);
    expect(header.text())
      .toEqual('translate_myAccount.aboutUsHeading');
  });

  it('links available', () => {
    const links = wrapper.findAll('li');
    expect(links.at(0).text()).toContain('translate_myAccount.helpAndSupport');
    expect(links.at(1).text()).toContain('translate_myAccount.accessibilityStatement');
    expect(links.at(2).text()).toContain('translate_myAccount.openSourceLicences');
    expect(links.at(3).text()).toContain('translate_myAccount.privacyPolicy');
    expect(links.at(4).text()).toContain('translate_myAccount.termsAndConditions');
  });
});

