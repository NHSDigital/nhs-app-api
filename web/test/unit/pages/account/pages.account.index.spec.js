import AboutUs from '@/components/account/AboutUs';
import AccountPage from '@/pages/account/index';
import i18n from '@/plugins/i18n';
import WebFooter from '@/components/widgets/WebFooter';
import each from 'jest-each';
import { createStore, initFilters, mount } from '../../helpers';

describe('Account Page', () => {
  jest.mock('@/services/native-app');
  let wrapper;

  initFilters();

  let $state;

  beforeEach(() => {
    $state = {
      device: {
        isNativeApp: false,
      },
      serviceJourneyRules: {
        rules: {
          supportsLinkedProfiles: false,
        },
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
  });

  const buildStoreGetters = ({
    notificationsEnabled = false,
  } = {}) => ({
    'serviceJourneyRules/notificationsEnabled': notificationsEnabled,
    'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(true),
    'session/isProofLevel9': true,
    'serviceJourneyRules/silverIntegrationEnabled': jest.fn().mockReturnValue(false),
  });

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

  const mountPage = ({
    notificationsEnabled = false,
    isNativeApp = false,
    supportsLinkedProfiles = false,
  }) => {
    const $store = createStore({ state: $state });

    $store.getters = buildStoreGetters({
      notificationsEnabled,
    });

    $state.device.isNativeApp = isNativeApp;
    $state.serviceJourneyRules.rules.supportsLinkedProfiles = supportsLinkedProfiles;
    return mount(AccountPage, {
      $store,
      $state,
      $style: createStyle(),
      mountOpts: { i18n },
    });
  };

  describe('on a native app', () => {
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: true,
        supportsLinkedProfiles: true,
      });
    });

    it('will verify that footer links are subset of links in account page', () => {
      const webFooterWrapper = mount(WebFooter, { mountOpts: { i18n } });
      const aboutUsWrapper = mount(AboutUs, { mountOpts: { i18n } });

      const footerLinkElements = webFooterWrapper.findAll('ul li a');
      const accountLinkElements = aboutUsWrapper.findAll('ul li a');

      const footerLinks = findLinks(footerLinkElements.wrappers);
      const accountLinks = findLinks(accountLinkElements.wrappers);
      expect(wrapper.find(AboutUs).exists()).toBe(true);
      expect(accountLinks.length).toBeGreaterThan(0);
      expect(footerLinks.every(link => accountLinks.includes(link))).toBeTruthy();
    });

    it('will have a linked profiles link', () => {
      expect(wrapper.findAll('li').at(0).text()).toContain('Linked profiles');
    });

    it('will have a cookies link', () => {
      expect(wrapper.findAll('li').at(1).text()).toContain('Cookies');
    });

    it('will show About the NHS App component', () => {
      expect(wrapper.find(AboutUs).exists()).toBe(true);
    });
  });

  describe('linked profiles link', () => {
    each([
      ['shown', 'supports linked profiles', true, true],
      ['hidden', 'does not support linked profiles', false, false],
    ])
      .it('will be %s when the gp %s', (_, __, supportsLinkedProfiles, isVisible) => {
        wrapper = mountPage({ supportsLinkedProfiles });
        expect(wrapper.find('#linked-profiles-link').exists()).toBe(isVisible);
      });
  });
});
