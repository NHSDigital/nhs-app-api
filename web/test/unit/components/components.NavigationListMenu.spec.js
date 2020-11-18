import NavigationListMenu from '@/components/NavigationListMenu';
import each from 'jest-each';
import { mount, createStore, createRouter } from '../helpers';

let wrapper;
let $store;
let $router;
let goToUrl;

const mountAs = ({
  isNativeApp = false,
  isProofLevel9 = true,
  gpMessagingSessionUnavailable = false,
  hasLinkedAccounts = true,
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
      'session/isProofLevel9': isProofLevel9,
      'linkedAccounts/hasLinkedAccounts': hasLinkedAccounts,
    },
  });
  return mount(NavigationListMenu, { $store, $router, methods: { goToUrl } });
};

beforeEach(() => {
  goToUrl = jest.fn();
  wrapper = mountAs();
  window.open = jest.fn();
});

describe('Navigation Links ', () => {
  describe('Messages Hub link', () => {
    each([
      ['shown', 'gpMessagingSession is available', false, true],
      ['hidden', 'gpMessagingSession is unavailable', true, false],
    ])
      .it('messages hub link will be %s when user %s', (_linkState, _gpMessagingSesstionState, gpMessagingSessionUnavailable, isVisible) => {
        wrapper = mountAs({ gpMessagingSessionUnavailable });
        expect(wrapper.find('#btn_messages').exists()).toBe(isVisible);
      });
  });

  describe('Linked Accounts link', () => {
    each([
      ['shown', 'has linked accounts', true, true],
      ['hidden', 'does not have linked accounts', false, false],
    ])
      .it('linked accounts link will be %s when user %s', (_linkState, _linkedAccountsState, hasLinkedAccounts, isVisible) => {
        wrapper = mountAs({ hasLinkedAccounts });
        expect(wrapper.find('#linked-profiles-link').exists()).toBe(isVisible);
      });
    it('will dispatch to set the breadcrumb to the default', () => {
      wrapper = mountAs();
      wrapper.vm.navigateToLinkedProfiles();

      expect($store.dispatch).toBeCalledWith('navigation/setRouteCrumb', 'defaultCrumb');
    });
  });

  describe('P9 User', () => {
    beforeEach(() => {
      wrapper = mountAs({ isProofLevel9: true });
    });
    it('will show prescriptions link', () => {
      expect(wrapper.find('#menu-item-prescriptions').exists()).toBe(true);
    });
    it('will show GP Health record link', () => {
      expect(wrapper.find('#menu-item-myRecord').exists()).toBe(true);
    });
  });

  describe('P5 User', () => {
    beforeEach(() => {
      wrapper = mountAs({ isProofLevel9: false });
    });
    it('will not show prescriptions link', () => {
      expect(wrapper.find('#menu-item-prescriptions').exists()).toBe(false);
    });
    it('will not show GP Health record link', () => {
      expect(wrapper.find('#menu-item-myRecord').exists()).toBe(false);
    });
  });
});
