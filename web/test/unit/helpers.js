/* eslint-disable import/prefer-default-export */
// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import Vuex from 'vuex';
import merge from 'lodash/fp/merge';
import { createLocalVue, mount as vueMount, shallowMount as vueShallowMount } from '@vue/test-utils';


export const $t = jest.fn().mockImplementation(key => `translate_${key}`);
export const $tc = $t;

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
} = {}) => ({
  app: {
    $cookies,
    $env,
    $http,
    context,
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

export const mount = (component, {
  $cookies,
  $env = {},
  $route = {},
  $router = [],
  $store,
  $style = {},
  t = $t,
  data = undefined,
  propsData = {},
  shallow = false,
  state = {},
  stubs = [],
} = {}) => {
  const store = $store || createStore({ $env, state });
  const localVue = createLocalVue();
  localVue.use(Vuex);

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
      $t: t,
      $tc,
      showTemplate: () => true,
    },
    stubs,
  });
};

export const shallowMount = (component, options = {}) =>
  mount(component, merge(options, { shallow: true }));

export const toClass = style => `.${style}`;
