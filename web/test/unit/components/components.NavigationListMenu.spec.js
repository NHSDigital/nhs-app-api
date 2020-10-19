import NavigationListMenu from '@/components/NavigationListMenu';
import each from 'jest-each';
import { mount, createStore, createRouter } from '../helpers';

jest.mock('@/lib/utils');

let wrapper;
let $store;
let $router;

const mountAs = ({
  silverIntegrationsEnabled = false,
  isNativeApp = false,
  isLinkedEnabledEnabled = false,
  isProofLevel9 = true,
  gpMessagingSessionUnavailable = false,
} = {}) => {
  $router = createRouter();
  $store = createStore({
    state: {
      device:
        {
          isNativeApp,
        },
      gpMessages:
      {
        gpMessagingSessionUnavailable,
      },
    },
    getters: {
      'linkedAccounts/hasLinkedAccounts': isLinkedEnabledEnabled,
      'session/isProofLevel9': isProofLevel9,
      'serviceJourneyRules/silverIntegrationEnabled': () => (silverIntegrationsEnabled),
    },
  });
  return mount(NavigationListMenu, { $store, $router });
};

beforeEach(() => {
  wrapper = mountAs();
  window.open = jest.fn();
});

describe('Navigation Links ', () => {
  describe('linked profiles visiblitiy', () => {
    it('shows Linked profiles link', () => {
      wrapper = mountAs({ isLinkedEnabledEnabled: true });
      expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(true);
    });

    it('does not show Linked profiles link', () => {
      wrapper = mountAs({ isLinkedEnabledEnabled: false });
      expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(false);
    });

    it('will not show Linked profiles link when is not a p9 user', () => {
      wrapper = mountAs({ isLinkedEnabledEnabled: true, isProofLevel9: false });
      expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(false);
    });
  });

  describe('Messages Hub link', () => {
    each([
      [false, true],
      [true, false],
    ])
      .it('messages hub link will be shown', (gpMessagingSessionUnavailable, isVisible) => {
        wrapper = mountAs({ silverIntegrationsEnabled: true, gpMessagingSessionUnavailable });
        expect(wrapper.find('#btn_messages').exists()).toBe(isVisible);
      });
  });

  describe('P9 User', () => {
    beforeEach(() => {
      wrapper = mountAs({ isProofLevel9: true });
    });

    it('will show appointments link', () => {
      expect(wrapper.find('#menu-item-appointments').exists()).toBe(true);
    });

    it('will show prescriptions link', () => {
      expect(wrapper.find('#menu-item-prescriptions').exists()).toBe(true);
    });

    it('will show advice link', () => {
      expect(wrapper.find('#menu-item-advice').exists()).toBe(true);
    });
  });

  describe('P5 User', () => {
    beforeEach(() => {
      wrapper = mountAs({ isProofLevel9: false });
    });

    it('will not show appointments link', () => {
      expect(wrapper.find('#menu-item-appointments').exists()).toBe(false);
    });

    it('will not show prescriptions link', () => {
      expect(wrapper.find('#menu-item-prescriptions').exists()).toBe(false);
    });

    it('will show advice link', () => {
      expect(wrapper.find('#menu-item-advice').exists()).toBe(true);
    });
  });

  describe('Health Record Links', () => {
    describe('Gp Health Record link', () => {
      it('GP Health record Link will be shown', () => {
        expect(wrapper.find('#menu-item-health-record-hub').exists()).toBe(false);
        expect(wrapper.find('#menu-item-myRecord').exists()).toBe(true);
      });
    });

    describe('Health Record Hub link', () => {
      it('health record hub link will be shown', () => {
        wrapper = mountAs({ silverIntegrationsEnabled: true });
        expect(wrapper.find('#menu-item-health-record-hub').exists()).toBe(true);
        expect(wrapper.find('#menu-item-myRecord').exists()).toBe(false);
      });
    });
  });
});
