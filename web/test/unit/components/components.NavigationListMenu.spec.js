import NavigationListMenu from '@/components/NavigationListMenu';
import { mount, createStore, createRouter, createEvent } from '../helpers';

let wrapper;
let $store;
let $router;

const mountAs = ({
  isNativeApp = false,
  isLinkedEnabledEnabled = false,
} = {}) => {
  $router = createRouter();
  $store = createStore({
    state: { device: { isNativeApp } },
    getters: { 'linkedAccounts/hasLinkedAccounts': isLinkedEnabledEnabled },
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
});
