/* eslint-disable import/no-extraneous-dependencies import/imports-first */
import Vuex from 'vuex';
import { shallowMount, createLocalVue } from '@vue/test-utils';

jest.mock('@/components/widgets/HotJar', () => {
});
/* eslint-disable import/first */
import DefaultPage from '@/layouts/default';

const $t = key => `translate_${key}`;
const $tc = key => `translate_${key}`;

const createDefaultPage = ($store, data = []) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  DefaultPage.components.HotJar = {
    computed: {},
    staticRenderFns: [],
    name: 'HotJar',
  };

  const $style = {};
  const $env = {};
  const $route = {
    query: '',
  };

  return shallowMount(DefaultPage, {
    localVue,
    data,
    mocks: {
      $http,
      $store,
      $env,
      $route,
      $t,
      $tc,
      $style,
      showTemplate: () => true,
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
  state: {
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

    createDefaultPage($store, {});
    expect($store.dispatch)
      .toHaveBeenLastCalledWith('auth/nativeLogin');
  });

  it('will not dispatch native login message when native', () => {
    const $store = createStore(false);

    jest.spyOn($store, 'dispatch');

    createDefaultPage($store, {});
    expect($store.dispatch)
      .not
      .toHaveBeenLastCalledWith('auth/nativeLogin');
  });
});
