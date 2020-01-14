/* eslint-disable import/no-extraneous-dependencies import/imports-first */
import ContentHeader from '@/components/widgets/ContentHeader';
import { create$T, createStore, shallowMount } from '../helpers';

const $t = create$T();

/* eslint-disable import/first */
import TsAndCsPage from '@/layouts/termsAndConditions';

const $http = jest.fn();
const $env = {};
const $style = {};

const createDefaultPage = ($store) => {
  const $route = {
    query: '',
    name: 'terms-and-conditions',
    helpUrl: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',
  };
  const loggedIn = true;
  return shallowMount(TsAndCsPage, {
    $http,
    $store,
    $env,
    $route,
    $t,
    $style,
    showTemplate: () => true,
    loggedIn,
    methods: {
      configureWebContext(url) {
        return url;
      },
    },
    stubs: {
      nuxt: '<div></div>',
    },
  });
};

const createLayoutStore = isNativeApp => createStore({
  $env: {
    VERSION_TAG: 1,
  },
  state: {
    appVersion: {
      webVersion: '1.2.3',
      nativeVersion: '3.2.1',
    },
    header: {
      headerText: 'someheader',
    },
    device: {
      isNativeApp,
    },
  },
});

describe('termsAndConditions.vue ', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {
    };
  });

  it('will show content header', () => {
    const $store = createLayoutStore(true);
    const defaultPage = createDefaultPage($store);

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
  });

  describe('mounted()', () => {
    it('will send correct help URL to setHelpUrl mixin function', () => {
      const $store = createLayoutStore(true);
      const defaultPage = createDefaultPage($store);
      const expectedHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

      expect(defaultPage.vm.currentHelpUrl)
        .toBe(expectedHelpUrl);
    });
  });

  describe('created()', () => {
    it('will dispatch appVersion/updateWebVersion on created', () => {
      const $store = createLayoutStore(true);
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenLastCalledWith('appVersion/updateWebVersion', 1);
    });

    it('will dispatch session/updateLastCalledAt if process is browser', () => {
      process.browser = true;
      const $store = createLayoutStore(true);
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenCalledWith('session/updateLastCalledAt');
    });

    it('will not dispatch session/updateLastCalledAt if process is not browser', () => {
      process.browser = false;
      const $store = createLayoutStore(true);
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .not.toHaveBeenCalledWith('session/updateLastCalledAt');
    });
  });
});

