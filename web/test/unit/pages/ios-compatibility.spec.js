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

  describe('content', () => {
    describe('incompatible', () => {
      beforeEach(() => {
        const query = { incompatible: 'true' };
        wrapper = mountPage({ query });
      });
      it('will show the correct paragraphs when the incompatible query is true', () => {
        const paragraphs = wrapper.findAll('p');
        expect(paragraphs.at(0).text()).toEqual('It requires iOS 11.0 or later, which is not compatible with this device.');
        expect(paragraphs.at(1).text()).toContain('If you have already registered for the app and proved who you are, you can access ');
      });
    });

    describe('compatible', () => {
      beforeEach(() => {
        const query = { incompatible: 'false' };
        wrapper = mountPage({ query });
      });

      it('will show the correct paragraphs when the incompatible query is false', () => {
        const paragraphs = wrapper.findAll('p');
        expect(paragraphs.at(0).text()).toEqual('To continue using the NHS App, you\'ll need to update your software version.');
        expect(paragraphs.at(1).text()).toContain('If you have already registered for the app and proved who you are, you can access ');
        expect(paragraphs.at(2).text()).toContain('If you have not registered for the app and proved who you are, you’ll need to:');
      });

      it('will show the correct list items when the incompatible query is false', () => {
        const query = { incompatible: 'false' };
        wrapper = mountPage({ query });
        const listItems = wrapper.findAll('li');
        expect(listItems.at(0).text()).toEqual('Update your software version on your device.');
        expect(listItems.at(1).text()).toContain('Register in the app.');
      });
    });
  });

  it('will redirect if it is an android device', () => {
    dependency.redirectTo = jest.fn();
    const query = { incompatible: 'true' };
    wrapper = mountPage({ query, source: 'android' });

    expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_PATH);
  });

  it('will redirect if there is no query', () => {
    dependency.redirectTo = jest.fn();
    wrapper = mountPage({ source: 'android' });

    expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_PATH);
  });
});
