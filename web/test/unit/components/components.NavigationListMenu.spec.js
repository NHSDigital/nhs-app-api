import NavigationListMenu from '@/components/NavigationListMenu';
import each from 'jest-each';
import { mount, createStore, createRouter } from '../helpers';

jest.mock('@/lib/utils');

let wrapper;
let $store;
let $router;

const mountAs = ({
  isNativeApp = false,
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
      'session/isProofLevel9': isProofLevel9,
    },
  });
  return mount(NavigationListMenu, { $store, $router });
};

beforeEach(() => {
  wrapper = mountAs();
  window.open = jest.fn();
});

describe('Navigation Links ', () => {
  describe('Messages Hub link', () => {
    each([
      [false, true],
      [true, false],
    ])
      .it('messages hub link will be shown', (gpMessagingSessionUnavailable, isVisible) => {
        wrapper = mountAs({ gpMessagingSessionUnavailable });
        expect(wrapper.find('#btn_messages').exists()).toBe(isVisible);
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
