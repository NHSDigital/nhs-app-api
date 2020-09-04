import NavigationListMenu from '@/components/NavigationListMenu';
import { redirectTo } from '@/lib/utils';
import { HEALTH_INFORMATION_UPDATES_PATH, MESSAGES_PATH } from '@/router/paths';
import { mount, createStore, createRouter } from '../helpers';

jest.mock('@/lib/utils');

let wrapper;
let $store;
let $router;
let propsData;

const mountAs = ({
  silverIntegrationsEnabled = false,
  isNativeApp = false,
  isLinkedEnabledEnabled = false,
  isProofLevel9 = true,
  linkToAppMessages = false,
} = {}) => {
  $router = createRouter();
  propsData = {
    linkToAppMessages,
  };
  $store = createStore({
    state: {
      device:
        {
          isNativeApp,
        },
    },
    getters: {
      'linkedAccounts/hasLinkedAccounts': isLinkedEnabledEnabled,
      'session/isProofLevel9': isProofLevel9,
      'serviceJourneyRules/silverIntegrationEnabled': () => (silverIntegrationsEnabled),
    },
  });
  return mount(NavigationListMenu, { $store, $router, propsData });
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

  describe('messaging link', () => {
    it('will display a link to the messages hub', () => {
      wrapper = mountAs();
      expect(wrapper.find('#btn_messages').exists()).toBe(true);
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

  describe('Messages Link', () => {
    describe('linkToAppMessages is true', () => {
      beforeEach(() => {
        wrapper = mountAs({ linkToAppMessages: true });
        wrapper.vm.navigateToMessages();
      });

      it('will redirect to app messages', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
      });

      it('will set route crumb to appMessagesOnlyCrumb', () => {
        expect($store.dispatch).toHaveBeenCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
      });
    });

    describe('linkToAppMessages is false', () => {
      beforeEach(() => {
        wrapper = mountAs();
        wrapper.vm.navigateToMessages();
      });

      it('will redirect to app messages', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, MESSAGES_PATH);
      });

      it('will set route crumb to appMessagesOnlyCrumb', () => {
        expect($store.dispatch).not.toHaveBeenCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
      });
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
