import { createStore, mount } from '../helpers';
import get from 'lodash/fp/get';
import NativeCallbacks from '@/services/native-app';
import ResetPageFocusMixin from '@/plugins/mixinDefinitions/ResetPageFocus';

jest.mock('lodash/fp/get');

describe('resetPageFocus mounted mixin', () => {
  let $store;
  let $route;
  let spy;

  beforeEach(() => {
    jest.clearAllMocks();
    $store = {};
    $route = {
      path: '/test-url-path',
    };
    spy = jest.spyOn(NativeCallbacks, 'resetPageFocus');
  });

  it('will call NativeCallbacks.resetPageFocus when client side, native, page loads', () => {
    // arrange
    const component = {
      template: '<div></div>',
      mixins: [ResetPageFocusMixin],
    };

    const keyPathFunction = jest.fn().mockImplementation(() => '/test-url-path');

    get.mockImplementation(() => keyPathFunction);

    process.client = true;

    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
      },
    });

    // act
    mount(component, { $store, $route });

    // assert
    expect(spy).toHaveBeenCalled();
  });

  it('will not call NativeCallbacks.resetPageFocus when not native app', () => {
    // arrange
    const component = {
      template: '<div></div>',
      mixins: [ResetPageFocusMixin],
    };

    process.client = true;

    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });

    // act
    mount(component, { $store, $route });

    // assert
    expect(spy).not.toHaveBeenCalled();
  });

  it('will not call NativeCallbacks when running server side', () => {
    // arrange
    const component = {
      template: '<div></div>',
      mixins: [ResetPageFocusMixin],
    };

    process.client = false;

    // act
    mount(component, { $store, $route });

    // assert
    expect(spy).not.toHaveBeenCalled();
  });

  it('will not call NativeCallbacks.resetPageFocus when not a page', () => {
    // arrange
    const component = {
      template: '<div></div>',
      mixins: [ResetPageFocusMixin],
    };

    process.client = true;

    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
      },
    });

    const keyPathFunction = jest
      .fn()
      .mockImplementation(() => '/not-matching-test-url-path');

    get.mockImplementation(() => keyPathFunction);

    // act
    mount(component, { $store, $route });

    // assert
    expect(spy).not.toHaveBeenCalled();
  });
});
