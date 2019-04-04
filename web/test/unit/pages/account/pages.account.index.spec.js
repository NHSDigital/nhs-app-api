import AccountPage from '@/pages/account/index';
import WebFooter from '@/components/widgets/WebFooter';
import { mount, createStore, toClass } from '../../helpers';

describe('Account Page', () => {
  let wrapper;

  const $env = {
    TERMS_AND_CONDITIONS_URL: 'https://terms-and-conditions',
    COOKIES_POLICY_URL: 'https://cookies-policy',
    OPEN_SOURCE_LICENCES_URL: 'https://open-source_lincences',
    PRIVACY_POLICY_URL: 'https://privacy-policy',
    HELP_AND_SUPPORT_URL: 'https://help-and-support',
    ACCESSIBILITY_STATEMENT: 'https://accessibility-statement',
    BIOMETRICS_ENABLED: false,
  };

  const $state = {
    device: {
      isNativeApp: false,
    },
    session: {
      user: 'User',
      dateOfBirth: '5/4/2019',
      nhsNumber: '12346778',
    },
    appVersion: {
      webVersion: 'web',
    },
  };

  const createStyle = () => ({
    'list-menu': 'list-menu',
  });

  const findLinks = (linkWrappers = []) => {
    const urls = [];
    linkWrappers.forEach((linkWrapper) => {
      const link = linkWrapper.element.getAttribute('href');
      urls.push(link);
    });
    return urls;
  };

  beforeEach(() => {
    wrapper = mount(AccountPage, { $env,
      $store: createStore({ state: $state }),
      $state,
      $style: createStyle() });
  });

  it('will verify that footer links are subset of links in account page', () => {
    const webHeaderWrapper = mount(WebFooter, { $env });
    const accountCssLinkPath = `ul${toClass('list-menu')} li a`;

    const footerLinkElements = webHeaderWrapper.findAll('ul li a');
    const accountLinkElements = wrapper.findAll(accountCssLinkPath);

    const footerLinks = findLinks(footerLinkElements.wrappers);
    const accountLinks = findLinks(accountLinkElements.wrappers);
    expect(accountLinks.length).toBeGreaterThan(0);

    expect(footerLinks.every(link => accountLinks.includes(link))).toBeTruthy();
  });
});
