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
      },
      getters: {
        'serviceJourneyRules/notificationsEnabled': false,
        'linkedAccounts/hasLinkedAccounts': false,
        'appVersion/isNativeVersionAfter': () => true,
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
        .toBe(false);

      expect(wrapper.find('[data-purpose=logout-button]').exists())
        .toBe(false);
    });
  });
});
