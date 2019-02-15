/* eslint-disable import/prefer-default-export */
// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import Vuex from 'vuex';
import merge from 'lodash/fp/merge';
import testUtils from '@vue/test-utils';

const {
  createLocalVue,
  mount: vueMount,
  shallowMount: vueShallowMount,
} = testUtils;

export const $t = jest.fn().mockImplementation(key => `translate_${key}`);
export const $tc = key => `translate_${key}`;

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

export const createStore = ({ $env = {}, $http = {}, state = {} } = {}) => ({
  dispatch: jest.fn(),
  state,
  app: {
    $env,
    $http,
  },
});

export const initFilters = () => [
  'longDate',
].map(filter => Vue.filter(filter, () => {}));

export const mount = (component, {
  $env = {},
  $router = [],
  $store,
  $style = {},
  data = {},
  propsData = {},
  shallow = false,
  state = {},
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
      $router,
      $store: store,
      $t,
      $tc,
      $style,
      showTemplate: () => true,
    },
  });
};

export const shallowMount = (component, options = {}) =>
  mount(component, merge(options, { shallow: true }));
