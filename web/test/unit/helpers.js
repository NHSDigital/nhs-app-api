// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import Vuex from 'vuex';
import '@/plugins/directives';
import _locale from '@/locale/en';
import get from 'lodash/fp/get';
import has from 'lodash/fp/has';
import isString from 'lodash/fp/isString';
import merge from 'lodash/fp/merge';
import { formatDate } from '@/plugins/filters';
import {
  createLocalVue,
  mount as vueMount,
  shallowMount as vueShallowMount,
} from '@vue/test-utils';

const localVue = createLocalVue();
localVue.use(Vuex);

export const create$T = () => jest
  .fn()
  .mockImplementation((key) => {
    const value = get(key)(_locale);
    if (isString(value) || value === undefined) {
      return `translate_${key}`;
    }

    return value;
  });

export const create$Te = () => jest
  .fn()
  .mockImplementation(key => has(key)(_locale));

export const mockCookies = () => ({
  get: jest.fn(),
  set: jest.fn(),
  remove: jest.fn(),
});

export const createEvent = event => ({ preventDefault: jest.fn(), ...event });

export const createRouter = () => ({
  go: jest.fn(),
  goBack: jest.fn(),
  push: jest.fn(),
});

export const createStore = ({
  $cookies = {},
  $env = {},
  $http = {},
  getters = {},
  context = {},
  state = {},
  i18n = {
    t: create$T(),
    tc: create$T(),
    te: create$Te(),
  },
  router = {},
} = {}) => ({
  app: {
    $analytics: {
      logicError: jest.fn(),
    },
    $cookies,
    $env,
    $http,
    context,
    i18n,
    router,
  },
  dispatch: jest.fn(),
  getters,
  state,
});

export const createScrollTo = () => {
  const scrollTo = jest.fn();
  global.scrollTo = scrollTo;
  return scrollTo;
};

export const initFilters = () => [
  'longDate',
].map(filter => Vue.filter(filter, value => value));

Vue.filter('formatDate', formatDate);

export const locale = _locale;

export const mount = (component, {
  $cookies,
  $env = {},
  $route = {},
  $router = [],
  $store,
  $style = {},
  $t = create$T(),
  $tc = create$T(),
  $te = create$Te(),
  data = () => ({}),
  propsData = {},
  shallow = false,
  state = {},
  stubs = [],
  methods = {},
  slots = {},
} = {}) => {
  const store = $store || createStore({ $env, state });
  const mountFn = shallow ? vueShallowMount : vueMount;
  return mountFn(component, {
    localVue,
    data,
    propsData,
    mocks: {
      $cookies,
      $route,
      $router,
      $store: store,
      $style,
      $env,
      $t,
      $tc,
      $te,
      $i18n: {
        t: create$T(),
        tc: create$T(),
        te: create$Te(),
      },
      showTemplate: () => true,
    },
    stubs,
    methods,
    slots,
  });
};

export const shallowMount = (component, options = {}) =>
  mount(component, merge(options, { shallow: true }));

export const toClass = style => `.${style}`;
