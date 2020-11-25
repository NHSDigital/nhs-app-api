import i18n from '@/plugins/i18n';
import Index from '@/pages/account/index';
import each from 'jest-each';
import { createStore, mount } from '../../helpers';

describe('Account.index', () => {
  let wrapper;
  let $store;

  const mountIndex = ({
    isNativeApp = false,
    supportsLinkedProfiles,
    isProofLevel9 = true,
    silverIntegrationEnabled = true,
  } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp,
        },
        serviceJourneyRules: {
          rules: {
            supportsLinkedProfiles,
          },
        },
        appVersion: {
          nativeVersion: 1,
        },
      },
      getters: {
        'appVersion/isNativeVersionAfter': () => true,
        'knownServices/matchOneById': id => ({
          id,
          url: 'www.url.com',
        }),
        'serviceJourneyRules/notificationsEnabled': false,
        'serviceJourneyRules/silverIntegrationEnabled': jest.fn().mockImplementation(() => silverIntegrationEnabled),
        'session/isProofLevel9': isProofLevel9,
      },
    });
    return mount(Index, {
      $store,
      stubs: {
        'welcome-section': '<div></div>',
        settings: '<div data-purpose="setting-section"></div>',
        // 'analytics-tracked-tag': '<a></a>',
      },
      mountOpts: {
        i18n,
      },
    });
  };

  describe('Logout button', () => {
    it('should be visible in native view', () => {
      wrapper = mountIndex({ isNativeApp: true });

      expect(wrapper.find('[data-purpose=setting-section]').exists())
        .toBe(true);

      expect(wrapper.find('[data-purpose=logout-button]').text())
        .toBe('Log out');
    });

    it('should not be visible in desktop view', () => {
      wrapper = mountIndex();

      expect(wrapper.find('[data-purpose=setting-section]').exists())
        .toBe(true);

      expect(wrapper.find('[data-purpose=logout-button]').exists())
        .toBe(false);
    });
  });

  describe('Page loaded', () => {
    it('Substrakt participation SJR rule is checked', () => {
      wrapper = mountIndex({ nativeLoginOptionsMethodExists: true });
      expect($store.getters['serviceJourneyRules/silverIntegrationEnabled'])
        .toHaveBeenCalledWith({ provider: 'substraktPatientPack', serviceType: 'participation' });
    });
  });

  describe('Patient Participation panel', () => {
    let patientParticipationPanel;
    it('should be visible', () => {
      wrapper = mountIndex({ nativeLoginOptionsMethodExists: true });
      patientParticipationPanel = wrapper.find('#btn_substrakt_participation');
      expect(patientParticipationPanel.exists()).toBe(true);
    });

    it('should not be visible if silver integration is not enabled', () => {
      wrapper = mountIndex({
        nativeLoginOptionsMethodExists: true,
        silverIntegrationEnabled: false,
      });

      patientParticipationPanel = wrapper.find('#btn_substrakt_participation');
      expect(patientParticipationPanel.exists()).toBe(false);
    });

    it('should not be visible if not proof level 9', () => {
      wrapper = mountIndex({
        nativeLoginOptionsMethodExists: true,
        isProofLevel9: false,
        silverIntegrationEnabled: true,
      });
      patientParticipationPanel = wrapper.find('#btn_substrakt_participation');
      expect(patientParticipationPanel.exists()).toBe(false);
    });
  });

  describe('linked accounts link', () => {
    each([
      ['shown', true, true],
      ['hidden', false, false],
    ])
      .it('will be %s if supportsLinkedProfiles is %s', (_, supportsLinkedProfiles, expected) => {
        wrapper = mountIndex({ supportsLinkedProfiles });

        expect(wrapper.find('#linked-profiles-link').exists())
          .toBe(expected);
      });
  });
});
