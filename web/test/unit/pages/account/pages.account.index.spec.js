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
    isProofLevel9 = true,
  } = {}) => ({
    'serviceJourneyRules/notificationsEnabled': notificationsEnabled,
    'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(true),
    'session/isProofLevel9': isProofLevel9,
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
    isProofLevel9 = true,
  }) => {
    const $store = createStore({ state: $state });

    $store.getters = buildStoreGetters({
      notificationsEnabled,
      isProofLevel9,
    });

    $store.app.$http = {
      postV1PatientAssertedLoginIdentity: jest.fn()
        .mockImplementation(() => Promise.resolve({ token: 'jwtToken' })),
    };

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
      ['shown', 'displays linked profiles if P9 and linked profiles are supported', true, true, true],
      ['hidden', 'does not display linked profiles if P9 and linked profiles are not supported', false, true, false],
      ['hidden', 'does not display linked profiles if P5', true, false, false],
      ['hidden', 'does not display linked profiles if P5 and linked profiles are not supported', false, false, false],
    ])
      .it('will be %s and %s', (_, __, supportsLinkedProfiles, isProofLevel9, isVisible) => {
        wrapper = mountPage({ supportsLinkedProfiles, isProofLevel9 });
        expect(wrapper.find('#linked-profiles-link').exists()).toBe(isVisible);
      });
  });
});
