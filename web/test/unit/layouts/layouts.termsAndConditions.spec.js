/* eslint-disable import/no-extraneous-dependencies import/imports-first */
import Vuex from 'vuex';
import ContentHeader from '@/components/widgets/ContentHeader';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import { create$T } from '../helpers';

const $t = create$T();

/* eslint-disable import/first */
import TsAndCsPage from '@/layouts/termsAndConditions';

const $http = jest.fn();
const $env = {};
const $style = {};
const localVue = createLocalVue();

const createDefaultPage = ($store) => {
  localVue.use(Vuex);
  localVue.mixin({
    methods: {
      configureWebContext(url) {
        return url;
      },
    },
  });
  const $route = {
    query: '',
    name: 'terms-and-conditions',
    helpUrl: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',
  };
  const loggedIn = true;
  return shallowMount(TsAndCsPage, {
    localVue,
    mocks: {
      $http,
      $store,
      $env,
      $route,
      $t,
      $style,
      showTemplate: () => true,
      loggedIn,
    },
    stubs: {
      nuxt: '<div></div>',
    },
  });
};

const createStore = isNativeApp => ({
  app: {
    $env: {
      VERSION_TAG: 1,
    },
  },
  dispatch: jest.fn(),
  subscribe: jest.fn(),
  state: {
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
    const $store = createStore(true);
    const defaultPage = createDefaultPage($store);

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
  });

  describe('mounted()', () => {
    it('will send correct help URL to setHelpUrl mixin function', () => {
      const $store = createStore(true);
      const defaultPage = createDefaultPage($store);
      const expectedHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

      expect(defaultPage.vm.currentHelpUrl)
        .toBe(expectedHelpUrl);
    });
  });

  describe('created()', () => {
    it('will dispatch appVersion/updateWebVersion on created', () => {
      const $store = createStore(true);
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenLastCalledWith('appVersion/updateWebVersion', 1);
    });

    it('will dispatch session/updateLastCalledAt if process is browser', () => {
      process.browser = true;
      const $store = createStore(true);
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .toHaveBeenCalledWith('session/updateLastCalledAt');
    });

    it('will not dispatch session/updateLastCalledAt if process is not browser', () => {
      process.browser = false;
      const $store = createStore(true);
      jest.spyOn($store, 'dispatch');

      createDefaultPage($store);
      expect($store.dispatch)
        .not.toHaveBeenCalledWith('session/updateLastCalledAt');
    });
  });
});

