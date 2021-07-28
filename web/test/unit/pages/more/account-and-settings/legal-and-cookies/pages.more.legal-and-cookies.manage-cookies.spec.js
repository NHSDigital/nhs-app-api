import ManageCookiesPage from '@/pages/more/account-and-settings/legal-and-cookies/manage-cookies';
import i18n from '@/plugins/i18n';
import { createStore, initFilters, mount } from '../../../../helpers';

describe('Manage Cookies Page', () => {
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
      termsAndConditions: {
        analyticsCookieAccepted: true,
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
    return mount(ManageCookiesPage, {
      $store,
      $state,
      $style: createStyle(),
      mountOpts: { i18n },
    });
  };

  function CookiePolicyLinkIsDisplayed() {
    it('will have a cookies policy link', () => {
      expect(wrapper.findAll('li').at(0).text()).toContain('Cookies policy');
    });
  }

  function AllowOptionalAnalyticCookiesIsChecked() {
    it('Allow optional analytic cookies will be checked', () => {
      const toggle = wrapper.find('input');
      expect(toggle.element.checked).toBeTruthy();
    });
  }

  function IsBackLinkPresent(testCaseName, isLinkPresent) {
    it(testCaseName, () => {
      expect(wrapper.find('[data-purpose=main-back-button]').exists()).toBe(isLinkPresent);
    });
  }

  describe('on a native app', () => {
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: true,
        supportsLinkedProfiles: true,
      });
    });

    CookiePolicyLinkIsDisplayed();
    AllowOptionalAnalyticCookiesIsChecked();
    IsBackLinkPresent('back link not present', false);
  });

  describe('on desktop', () => {
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: false,
        supportsLinkedProfiles: true,
      });
    });

    CookiePolicyLinkIsDisplayed();
    AllowOptionalAnalyticCookiesIsChecked();
    IsBackLinkPresent('back link present', true);
  });
});
