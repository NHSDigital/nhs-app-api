import LegalAndCookiesPage from '@/pages/more/account-and-settings/legal-and-cookies/index';
import i18n from '@/plugins/i18n';
import { createStore, initFilters, mount } from '../../../../helpers';

describe('Legal and Cookies Page', () => {
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

    $state.device.isNativeApp = isNativeApp;
    $state.serviceJourneyRules.rules.supportsLinkedProfiles = supportsLinkedProfiles;
    return mount(LegalAndCookiesPage, {
      $store,
      $state,
      $style: createStyle(),
      mountOpts: { i18n },
    });
  };

  function PageLinksAreDisplayed() {
    it('will have a manage cookies link', () => {
      expect(wrapper.findAll('li').at(0).text()).toContain('Manage cookies');
    });

    it('will have a terms of use link', () => {
      expect(wrapper.findAll('li').at(1).text()).toContain('Terms of use');
    });

    it('will have a privacy policy link', () => {
      expect(wrapper.findAll('li').at(2).text()).toContain('Privacy policy');
    });

    it('will have an accessibility statement link', () => {
      expect(wrapper.findAll('li').at(3).text()).toContain('Accessibility statement');
    });

    it('will have an open source licences link', () => {
      expect(wrapper.findAll('li').at(4).text()).toContain('Open source licences');
    });
  }

  describe('on a native app', () => {
    let backLink;
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: true,
        supportsLinkedProfiles: true,
      });

      backLink = wrapper.find('[data-purpose=back-link]');
    });

    PageLinksAreDisplayed();

    it('will not have a back link', () => {
      expect(backLink.exists()).toBe(false);
    });
  });

  describe('on desktop', () => {
    let backLink;
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: false,
        supportsLinkedProfiles: true,
      });

      backLink = wrapper.find('[data-purpose=back-link]');
    });

    PageLinksAreDisplayed();

    it('will have a back link', () => {
      expect(backLink.exists()).toBe(true);
    });
  });
});
