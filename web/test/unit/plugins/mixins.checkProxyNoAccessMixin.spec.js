import get from 'lodash/fp/get';
import CheckProxyNoAccessMixin from '@/plugins/mixinDefinitions/CheckProxyNoAccessMixin';
import { createStore, createRouter, mount } from '../helpers';
import { redirectTo } from '@/lib/utils';
import { findByName } from '@/lib/routes';

jest.mock('@/lib/utils');
jest.mock('@/lib/routes');
jest.mock('lodash/fp/get');

describe('CheckProxyNoAccessMixin mounted mixin', () => {
  let $store;
  let $router;
  let component;
  const currentRouteName = 'example';

  beforeEach(() => {
    jest.clearAllMocks();
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
      },
    });
    $router = createRouter();
    $router.currentRoute = {
      name: currentRouteName,
    };
    component = {
      template: '<div></div>',
      mixins: [CheckProxyNoAccessMixin],
    };
  });

  it('will redirect to shutter page if error 403 proxying and route has shutter page defined', () => {
    // arrange
    $store.getters['session/isProxying'] = true;
    $store.getters['errors/showApiError'] = true;
    $store.state = {
      errors: {
        apiErrors: [{ status: 403 }],
      },
    };

    const getFunction = jest.fn().mockImplementation(() => 403);
    get.mockImplementation(() => getFunction);

    const route = {
      name: 'example',
      proxyShutterPath: '/shutter/example',
    };
    findByName.mockImplementation(() => route);

    // act
    const wrapper = mount(component, { $store, $router });

    // assert
    expect(get).toHaveBeenCalledWith('$store.state.errors.apiErrors[0].status');
    expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/shutter/example');
  });

  it('will not redirect to shutter page if error 403, proxying but route does not have shutter page defined', () => {
    // arrange
    $store.getters['session/isProxying'] = true;
    $store.getters['errors/showApiError'] = true;
    $store.state = {
      errors: {
        apiErrors: [{ status: 403 }],
      },
    };

    const getFunction = jest.fn().mockImplementation(() => 403);
    get.mockImplementation(() => getFunction);

    const route = {
      name: 'example',
    };
    findByName.mockImplementation(() => route);

    // act
    mount(component, { $store, $router });

    // assert
    expect(get).toHaveBeenCalledWith('$store.state.errors.apiErrors[0].status');
    expect(redirectTo).not.toHaveBeenCalled();
  });

  it('will not redirect to shutter page if error 403 and not proxying', () => {
    // arrange
    $store.getters['session/isProxying'] = false;
    $store.getters['errors/showApiError'] = true;
    $store.state = {
      errors: {
        apiErrors: [{ status: 403 }],
      },
    };

    const getFunction = jest.fn().mockImplementation(() => 403);
    get.mockImplementation(() => getFunction);

    const route = {
      name: '/example',
      proxyShutterPath: '/shutter/example',
    };
    findByName.mockImplementation(() => route);

    // act
    mount(component, { $store, $router });

    // assert
    expect(get).not.toHaveBeenCalled();
    expect(redirectTo).not.toHaveBeenCalled();
  });

  it('will not redirect to shutter page if error is not 403, even when proxying', () => {
    // arrange
    $store.getters['session/isProxying'] = true;
    $store.getters['errors/showApiError'] = true;
    $store.state = {
      errors: {
        apiErrors: [{ status: 500 }],
      },
    };

    const getFunction = jest.fn().mockImplementation(() => 500);
    get.mockImplementation(() => getFunction);

    const route = {
      name: 'example',
      proxyShutterPath: '/shutter/example',
    };
    findByName.mockImplementation(() => route);

    // act
    mount(component, { $store, $router });

    // assert
    expect(get).toHaveBeenCalledWith('$store.state.errors.apiErrors[0].status');
    expect(redirectTo).not.toHaveBeenCalled();
  });
});
