import * as dependency from '@/lib/utils';
import IOSCompatibility from '@/pages/ios-compatibility';
import { LOGIN_PATH } from '@/router/paths';
import { createStore, shallowMount } from '../helpers';

describe('ios compatibility page', () => {
  let $store;
  let wrapper;

  const mountPage = ({
    isNativeApp = true,
    source = 'ios',
    query = {},
  } = {}) => {
    $store = createStore({
      state: {
        appVersion: {
          webVersion: '1.2.3',
          nativeVersion: '3.2.1',
        },
        device: {
          isNativeApp,
          source,
        },
      },
    });

    return shallowMount(IOSCompatibility, {
      $store,
      $route: {
        query,
        meta: {
          helpUrl: '',
        },
      },
      methods: {
        configureWebContext(url) {
          return url;
        },
      },
    });
  };

  it('will show the correct content when the incompatible query is true', () => {
    const query = { incompatible: true };
    wrapper = mountPage({ query });

    const paragraphs = wrapper.findAll('p');
    expect(paragraphs.at(0).text()).toEqual('It requires iOS 11.0 or later, which is not compatible with this device.');
    expect(paragraphs.at(1).text()).toContain('If you have already registered for the app and proved who you are, you can access ');
  });

  it('will redirect if it is an android device', () => {
    dependency.redirectTo = jest.fn();
    const query = { incompatible: true };
    wrapper = mountPage({ query, source: 'android' });

    expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_PATH);
  });

  it('will redirect if there is no query', () => {
    dependency.redirectTo = jest.fn();
    wrapper = mountPage({ source: 'android' });

    expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_PATH);
  });
});
