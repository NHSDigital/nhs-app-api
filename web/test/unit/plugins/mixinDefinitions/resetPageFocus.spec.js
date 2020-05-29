import get from 'lodash/fp/get';
import NativeCallbacks from '@/services/native-app';
import ResetPageFocusMixin from '@/plugins/mixinDefinitions/ResetPageFocus';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('lodash/fp/get');
jest.mock('@/services/event-bus');
jest.mock('@/services/native-app');

const $route = { path: '/test-url-path' };
const mountMixin = ({ isNativeApp }) => {
  const $store = createStore({
    state: {
      device: {
        isNativeApp,
      },
    },
  });
  mount({ template: '<div></div>', mixins: [ResetPageFocusMixin] }, { $store, $route });
};

describe('resetPageFocus mounted mixin', () => {
  describe('not a page loading', () => {
    beforeEach(() => {
      get.mockImplementation(() => jest.fn().mockImplementation(() => '/test-component-url-path'));
      mountMixin({ isNativeApp: true });
    });

    it('will not call `NativeCallbacks.resetPageFocus`', () => {
      expect(NativeCallbacks.resetPageFocus).not.toBeCalled();
    });

    it('will not emit', () => {
      expect(EventBus.$emit).not.toBeCalled();
    });
  });

  describe('page loading', () => {
    beforeEach(() => {
      get.mockImplementation(() => jest.fn().mockImplementation(() => $route.path));
    });

    describe('is native', () => {
      beforeEach(() => {
        mountMixin({ isNativeApp: true });
      });

      it('will call `NativeCallbacks.resetPageFocus`', () => {
        expect(NativeCallbacks.resetPageFocus).toHaveBeenCalled();
      });

      it('will emit `FOCUS_NHSAPP_ROOT`', () => {
        expect(EventBus.$emit).toBeCalledWith(FOCUS_NHSAPP_ROOT);
      });
    });

    describe('is not native', () => {
      beforeEach(() => {
        mountMixin({ isNativeApp: false });
      });

      it('will not call `NativeCallbacks.resetPageFocus`', () => {
        expect(NativeCallbacks.resetPageFocus).not.toBeCalled();
      });

      it('will emit `FOCUS_NHSAPP_ROOT`', () => {
        expect(EventBus.$emit).toBeCalledWith(FOCUS_NHSAPP_ROOT);
      });
    });
  });

  afterEach(() => {
    jest.resetAllMocks();
  });
});
