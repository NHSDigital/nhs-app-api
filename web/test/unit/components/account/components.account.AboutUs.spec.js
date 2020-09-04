import AboutUs from '@/components/account/AboutUs';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../../helpers';

describe('AboutUs', () => {
  let wrapper;
  let $store;

  const mountAboutUs = () => mount(AboutUs, { $store, mountOpts: { i18n } });

  beforeEach(() => {
    wrapper = mountAboutUs();
    $store = createStore();
  });

  it('will have a header', () => {
    const header = wrapper.find('h2');
    expect(header.exists()).toBe(true);
    expect(header.text())
      .toEqual('About the NHS App');
  });

  it('links available', () => {
    const links = wrapper.findAll('li');
    expect(links.at(0).text()).toContain('Help and support');
    expect(links.at(1).text()).toContain('Accessibility statement');
    expect(links.at(2).text()).toContain('Open source licences');
    expect(links.at(3).text()).toContain('Privacy policy');
    expect(links.at(4).text()).toContain('Terms of use');
  });
});
