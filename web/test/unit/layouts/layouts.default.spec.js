/* eslint-disable import/no-extraneous-dependencies import/imports-first */
import Vuex from 'vuex';
import ContentHeader from '@/components/widgets/ContentHeader';
import WebHeader from '@/components/widgets/WebHeader';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import { create$T } from '../helpers';

const $t = create$T();

jest.mock('@/components/widgets/HotJar', () => {
});
/* eslint-disable import/first */
import DefaultPage from '@/layouts/default';

const $http = jest.fn();
const $env = {};
const $style = {
  homeMain: true,
};
const localVue = createLocalVue();


const createDefaultPage = ($store) => {
  localVue.use(Vuex);
  localVue.mixin({
    methods: {
      configureWebContext(helpUrl) {
        return helpUrl;
      },
    },
  });
  DefaultPage.components.HotJar = {
    computed: {},
    staticRenderFns: [],
    name: 'HotJar',
  };
  const $route = {
    query: '',
    name: 'notLogin',
  };
  const loggedIn = true;
  return shallowMount(DefaultPage, {
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

const createDefaultPageForLoginScreen = ($store) => {
  localVue.use(Vuex);
  const $route = {
    query: '',
    name: 'Login',
  };
  const loggedIn = true;
  return shallowMount(DefaultPage, {
    localVue,
    mocks: {
      $http,
      $store,
      $env,
      $route,
      $style,
      $t,
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
  getters: {
    'errors/showApiError': false,
  },
  state: {
    header: {
      headerText: 'someheader',
    },
    device: {
      isNativeApp,
    },
    session: {
      csrfToken: 'someToken',
    },
    termsAndConditions: {
      analyticsCookieAccepted: true,
    },
  },
});


describe('default.vue - is native', () => {
  beforeEach(() => {
    process.client = true;
    window.validateSession = () => {
    };
  });

  it('will dispatch native login message when native', () => {
    const $store = createStore(true);
    jest.spyOn($store, 'dispatch');

    createDefaultPage($store);
    expect($store.dispatch)
      .toHaveBeenLastCalledWith('auth/nativeLogin');
  });

  it('will not dispatch native login message when native', () => {
    const $store = createStore(false);

    jest.spyOn($store, 'dispatch');

    createDefaultPage($store);
    expect($store.dispatch)
      .not
      .toHaveBeenLastCalledWith('auth/nativeLogin');
  });

  it('will show content header with breadcrumb when not in login page', () => {
    const $store = createStore(true);
    const defaultPage = createDefaultPage($store);

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
    expect(defaultPage.vm.shouldShowBreadCrumb).toBe(true);
  });

  it('will show content header without breadcrumb when in login page', () => {
    const $store = createStore(true);
    const defaultPage = createDefaultPageForLoginScreen($store);

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.find(ContentHeader).exists()).toBe(true);
    expect(defaultPage.vm.shouldShowBreadCrumb).toBe(false);
  });

  it('will show full desktop header when not native and not in login page', () => {
    const $store = createStore(false);
    const defaultPage = createDefaultPage($store);

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.vm.shouldShowFullDesktopHeader).toBe(true);
    expect(defaultPage.find(WebHeader).exists()).toBe(true);
  });

  it('will send correct help URL to setHelpUrl mixin function', () => {
    const $store = createStore(true);
    const defaultPage = createDefaultPage($store);
    const expectedHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/';

    jest.spyOn($store, 'dispatch');

    expect(defaultPage.vm.currentHelpUrl).toBe(expectedHelpUrl);
  });
});

