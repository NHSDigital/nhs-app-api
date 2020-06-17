import AboutUs from '@/components/account/AboutUs';
import { createStore, mount } from '../../helpers';

describe('AboutUs', () => {
  let wrapper;
  let $store;

  const mountAboutUs = () => mount(AboutUs, { $store });

  beforeEach(() => {
    wrapper = mountAboutUs();
    $store = createStore();
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
