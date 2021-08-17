import MorePage from '@/pages/more/index';
import i18n from '@/plugins/i18n';
import WebFooter from '@/components/widgets/WebFooter';
import each from 'jest-each';
import { TERMS_AND_CONDITIONS_URL, PRIVACY_POLICY_URL, ACCESSIBILITY_STATEMENT_URL } from '@/router/externalLinks';
import { createStore, initFilters, mount } from '../../helpers';

describe('More Page', () => {
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
    window.open = jest.fn();
  });

  const buildStoreGetters = ({
    isProofLevel9 = true,
  } = {}) => ({
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
    isNativeApp = false,
    supportsLinkedProfiles = false,
    isProofLevel9 = true,
  }) => {
    const $store = createStore({ state: $state });

    $store.getters = buildStoreGetters({
      isProofLevel9,
    });

    $store.app.$http = {
      postV1PatientAssertedLoginIdentity: jest.fn()
        .mockImplementation(() => Promise.resolve({ token: 'jwtToken' })),
    };

    $state.device.isNativeApp = isNativeApp;
    $state.serviceJourneyRules.rules.supportsLinkedProfiles = supportsLinkedProfiles;
    return mount(MorePage, {
      $store,
      $state,
      $style: createStyle(),
      mountOpts: { i18n },
    });
  };

  describe('on a native app', () => {
    beforeEach(() => {
      wrapper = mountPage({
        isNativeApp: true,
        supportsLinkedProfiles: true,
      });
    });

    it('will verify that the footer links are present', () => {
      const webFooterWrapper = mount(WebFooter, {
        mountOpts: { i18n },
        $env: { BASE_NHS_APP_HELP_URL: 'http://stubs.local.bitraft.io/help' },
      });
      const footerLinkElements = webFooterWrapper.findAll('ul li a');
      const footerLinks = findLinks(footerLinkElements.wrappers);

      expect(footerLinks.length).toBeGreaterThan(0);
      expect(footerLinks).toContainEqual(TERMS_AND_CONDITIONS_URL);
      expect(footerLinks).toContainEqual(PRIVACY_POLICY_URL);
      expect(footerLinks).toContainEqual('http://stubs.local.bitraft.io/help');
      expect(footerLinks).toContainEqual(ACCESSIBILITY_STATEMENT_URL);
    });

    it('will have a linked profiles link', () => {
      expect(wrapper.findAll('li').at(0).text()).toContain('Linked profiles');
    });

    it('will have an account and settings link', () => {
      expect(wrapper.findAll('li').at(1).text()).toContain('Account and settings');
    });

    it('will have a help and support link', () => {
      expect(wrapper.findAll('li').at(2).text()).toContain('Help and support');
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
