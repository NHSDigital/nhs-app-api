import Index from '@/pages/account/index';
import { createStore, mount } from '../../helpers';

describe('Account.index', () => {
  let wrapper;
  let store;
  let mountIndex;

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
      },
      getters: {
        'serviceJourneyRules/notificationsEnabled': false,
        'linkedAccounts/hasLinkedAccounts': false,
      },
    };

    mountIndex = ($store, $env) => mount(Index, {
      $env,
      $store: createStore($store),
      stubs: {
        'welcome-section': '<div></div>',
        settings: '<div data-purpose="setting-section"></div>',
        // 'analytics-tracked-tag': '<a></a>',
      },
    });
  });

  describe('Logout button', () => {
    it('should be visible as in native view', () => {
      store.state.device.isNativeApp = true;
      const env = {
        BIOMETRICS_ENABLED: true,
        nativeLoginOptionsMethodExists: true,
      };

      wrapper = mountIndex(store, env);

      expect(wrapper.find('[data-purpose=setting-section]').exists())
        .toBe(true);

      expect(wrapper.find('[data-purpose=logout-button]').text())
        .toBe('translate_signOutButton.signOut');
    });

    it('should not be visible as in desktop view', () => {
      store.state.device.isNativeApp = false;
      const env = {
        BIOMETRICS_ENABLED: true,
        nativeLoginOptionsMethodExists: true,
      };

      wrapper = mountIndex(store, env);

      expect(wrapper.find('[data-purpose=setting-section]').exists())
        .toBe(false);

      expect(wrapper.find('[data-purpose=logout-button]').exists())
        .toBe(false);
    });
  });
});
