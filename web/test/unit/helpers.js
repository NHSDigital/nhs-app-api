// eslint-disable-next-line import/no-extraneous-dependencies
import i18n from '@/plugins/i18n';
import merge from 'lodash/fp/merge';
import Vue from 'vue';
import Vuex from 'vuex';
import '@/plugins/directives';
import { formatDate } from '@/plugins/filters';
import {
  createLocalVue,
  mount as vueMount,
  shallowMount as vueShallowMount,
} from '@vue/test-utils';

export const localVue = createLocalVue();
localVue.use(Vuex);

export const mockCookies = () => ({
  get: jest.fn(),
  set: jest.fn(),
  remove: jest.fn(),
});

export const createEvent = event => ({ preventDefault: jest.fn(), ...event });

export const createRouter = (name = undefined) => ({
  go: jest.fn(),
  goBack: jest.fn(),
  push: jest.fn(),
  currentRoute: { name },
  history: {
    router: {
      previousPaths: [],
    },
  },
});

export const createStore = ({
  $cookies = {},
  $env = {},
  $http = {},
  getters = {},
  context = {},
  state = {},
  router = {},
} = {}) => ({
  app: {
    $analytics: {
      logicError: jest.fn(),
    },
    $http,
    context,
    i18n,
    router,
  },
  $cookies,
  dispatch: jest.fn(),
  getters,
  state,
  $env,
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

export const mount = (component, {
  $cookies,
  $env = {},
  $route = {},
  $router = [],
  $store,
  $style = {},
  data = () => ({}),
  mocks,
  propsData = {},
  shallow = false,
  state = {},
  stubs = [],
  methods = {},
  slots = {},
} = {}) => {
  const store = $store || createStore({ $env, state });
  const mountFn = shallow ? vueShallowMount : vueMount;

  const mocksObj = {
    $cookies,
    $route,
    $router,
    $store: store,
    $style,
    $env,
    showTemplate: () => true,
    ...mocks,
  };

  const options = {
    localVue,
    data,
    propsData,
    mocks: mocksObj,
    stubs,
    methods,
    slots,
    i18n,
  };

  return mountFn(component, options);
};

export const shallowMount = (component, options = {}) =>
  mount(component, merge(options, { shallow: true }));

export const toClass = style => `.${style}`;

export const normaliseWhiteSpace = value => (value.replace(/\s+/g, ' '));
