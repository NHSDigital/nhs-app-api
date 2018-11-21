/* eslint-disable import/prefer-default-export */
import Vuex from 'vuex';
import merge from 'lodash/fp/merge';
import testUtils from '@vue/test-utils';

const {
  createLocalVue,
  mount: vueMount,
  shallowMount: vueShallowMount,
} = testUtils;

export const $t = key => `translate_${key}`;
export const $tc = key => `translate_${key}`;

export const mockCookies = () => ({
  get: jest.fn(),
  set: jest.fn(),
  remove: jest.fn(),
});

export const createEvent = () => ({ preventDefault: jest.fn() });

export const createStore = ({ $http = {}, state = {} } = {}) => ({
  dispatch: jest.fn(),
  state,
  app: {
    $env: {},
    $http,
  },
});
export const mount = (component, {
  $router = [],
  $store,
  $style = {},
  state = {},
  data = {},
  propsData = {},
  shallow = false,
} = {}) => {
  const store = $store || createStore({ state });
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

