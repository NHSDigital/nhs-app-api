import AccountPage from '@/pages/account/index';
import AboutUs from '@/components/account/AboutUs';
import Settings from '@/components/account/Settings';
import WebFooter from '@/components/widgets/WebFooter';
import WelcomeSection from '@/components/WelcomeSection';
import { createStore, initFilters, mount, toClass } from '../../helpers';

describe('Account Page', () => {
  let wrapper;

  initFilters();

  const $env = {
    TERMS_AND_CONDITIONS_URL: 'https://terms',
    COOKIES_POLICY_URL: 'https://cookies',
    OPEN_SOURCE_LICENCES_URL: 'https://open-source',
    PRIVACY_POLICY_URL: 'https://privacy',
    HELP_AND_SUPPORT_URL: 'https://help',
    ACCESSIBILITY_STATEMENT: 'https://accessibility',
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

  const mountPage = ({ notificationsEnabled = false, isNativeApp = false }) => {
    const $store = createStore({ state: $state });
    $store.getters['serviceJourneyRules/notificationsEnabled'] = notificationsEnabled;
    $state.device.isNativeApp = isNativeApp;
    return mount(AccountPage, { $env,
      $store,
      $state,
      $style: createStyle() });
  };

  describe('not on a native app', () => {
    beforeEach(() => {
      wrapper = mountPage({ notificationsEnabled: true, isNativeApp: false });
    });

    it('will not show Settings component', () => {
      expect(wrapper.find(Settings).exists()).toBe(false);
    });
  });

  describe('on a native app', () => {
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

    it('will show About Us component', () => {
      expect(wrapper.find(AboutUs).exists()).toBe(true);
    });

    it('will show Welcome Section component', () => {
      expect(wrapper.find(WelcomeSection).exists()).toBe(true);
    });

    describe('service journey rules notifications disabled', () => {
      beforeEach(() => {
        wrapper = mountPage({ notificationsEnabled: false, isNativeApp: true });
      });

      it('will not show Settings component', () => {
        expect(wrapper.find(Settings).exists()).toBe(false);
      });
    });

    describe('service journey rules notifications enabled', () => {
      beforeEach(() => {
        wrapper = mountPage({ notificationsEnabled: true, isNativeApp: true });
      });

      it('will show Settings component', () => {
        expect(wrapper.find(Settings).exists()).toBe(true);
      });
    });
  });
});
