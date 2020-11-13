import i18n from '@/plugins/i18n';
import Index from '@/pages/account/index';
import { createStore, mount } from '../../helpers';

describe('Account.index', () => {
  let wrapper;
  let store;
  const mountIndex = ($store, $env) => mount(Index, {
    $env,
    $store: createStore(Object.assign({}, $store, $env)),
    stubs: {
      'welcome-section': '<div></div>',
      settings: '<div data-purpose="setting-section"></div>',
      // 'analytics-tracked-tag': '<a></a>',
    },
    mountOpts: {
      i18n,
    },
  });

  beforeEach(() => {
    store = {
      state: {
        device: {
          isNativeApp: false,
        },
        appVersion: {
          nativeVersion: 1,
        },
        session: {
          user: undefined,
        },
        knownServices: {
          knownServices: [{
            id: 'substraktPatientPack',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': jest.fn().mockImplementation(() => true),
        'serviceJourneyRules/notificationsEnabled': false,
        'linkedAccounts/hasLinkedAccounts': false,
        'appVersion/isNativeVersionAfter': () => true,
        'session/isProofLevel9': true,
      },
    };
  });

  describe('Logout button', () => {
    it('should be visible in native view', () => {
      store.state.device.isNativeApp = true;
      const env = {
        nativeLoginOptionsMethodExists: true,
      };

      wrapper = mountIndex(store, env);

      expect(wrapper.find('[data-purpose=setting-section]').exists())
        .toBe(true);

      expect(wrapper.find('[data-purpose=logout-button]').text())
        .toBe('Log out');
    });

    it('should not be visible in desktop view', () => {
      store.state.device.isNativeApp = false;
      const env = {
        nativeLoginOptionsMethodExists: true,
      };

      wrapper = mountIndex(store, env);

      expect(wrapper.find('[data-purpose=setting-section]').exists())
        .toBe(true);

      expect(wrapper.find('[data-purpose=logout-button]').exists())
        .toBe(false);
    });
  });

  describe('Page loaded', () => {
    it('Substrakt participation SJR rule is checked', () => {
      wrapper = mountIndex(store, { nativeLoginOptionsMethodExists: true });
      expect(store.getters['serviceJourneyRules/silverIntegrationEnabled'])
        .toHaveBeenCalledWith({ provider: 'substraktPatientPack', serviceType: 'participation' });
    });
  });

  describe('Patient Participation panel', () => {
    let patientParticipationPanel;
    it('should be visible', () => {
      wrapper = mountIndex(store, { nativeLoginOptionsMethodExists: true });
      patientParticipationPanel = wrapper.find('#btn_substrakt_participation');
      expect(patientParticipationPanel.exists()).toBe(true);
    });

    it('should not be visible if silver integration is not enabled', () => {
      store.getters['serviceJourneyRules/silverIntegrationEnabled'] = jest.fn().mockImplementation(() => false);
      store.getters['session/isProofLevel9'] = true;

      wrapper = mountIndex(store, { nativeLoginOptionsMethodExists: true });
      patientParticipationPanel = wrapper.find('#btn_substrakt_participation');
      expect(patientParticipationPanel.exists()).toBe(false);
    });

    it('should not be visible if not proof level 9', () => {
      store.getters['session/isProofLevel9'] = false;
      store.getters['serviceJourneyRules/silverIntegrationEnabled'] = jest.fn().mockImplementation(() => true);

      wrapper = mountIndex(store, { nativeLoginOptionsMethodExists: true });
      patientParticipationPanel = wrapper.find('#btn_substrakt_participation');
      expect(patientParticipationPanel.exists()).toBe(false);
    });
  });
});
