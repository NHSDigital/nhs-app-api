import NavigationListMenu from '@/components/NavigationListMenu';
import { mount, createStore, createRouter, createEvent } from '../helpers';

let wrapper;
let $store;
let $router;

const mountAs = ({
  isNativeApp = false,
  isLinkedEnabledEnabled = false,
  isProofLevel9 = true,
} = {}) => {
  $router = createRouter();
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
    },
    $env: {
      YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com',
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

  describe('messaging link', () => {
    it('will display a link to the messages hub', () => {
      wrapper = mountAs();
      expect(wrapper.find('#btn_messages').exists()).toBe(true);
    });
  });

  describe('methods', () => {
    describe('navigate', () => {
      it('will navigate to event current target path name', () => {
        wrapper.vm.navigate(createEvent({ currentTarget: { pathname: '/event/path' } }));
        expect($router.push).toHaveBeenCalledWith('/event/path');
      });
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

    it('will show symptoms link', () => {
      expect(wrapper.find('#menu-item-symptoms').exists()).toBe(true);
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

    it('will show symptoms link', () => {
      expect(wrapper.find('#menu-item-symptoms').exists()).toBe(true);
    });
  });
});
