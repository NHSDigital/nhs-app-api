/* eslint-disable import/extensions */
/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import each from 'jest-each';
import { mount, createLocalVue } from '@vue/test-utils';
import errors from '@/store/modules/errors';
import ApiError from '@/components/errors/ApiError';
import locale from '@/locale';
import { get, has } from 'lodash/fp';
import testData from './testData';

const engLocale = locale.en;
const $te = key => has(key, engLocale);
const $t = key => get(key, engLocale);
let component;

const createApiErrorComponent = ($route, apiError) => {
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const store = new Vuex.Store(errors);
  store.state.apiErrors = [apiError];
  store.getters['errors/showApiError'] = store.getters.showApiError;
  store.state.errors = store.state;

  component = mount(ApiError, {
    store,
    localVue,
    mocks: {
      $style: { serverError: 'error' },
      $route,
      $t,
      $te,
    },
  });
};

const assert = (data) => {
  expect(component.vm.getMessage('pageHeader')).toEqual(data.pageHeader);
  expect(component.find('#error').findAll('p').at(0).text()).toEqual(data.header);
  expect(component.find('#error').findAll('p').at(1).text()).toEqual(data.subheader);
  expect(component.find('#error').findAll('p').at(2).text()).toEqual(data.message);
  if (data.hasRetryButton) {
    expect(component.find('#error').find('.button').text()).toEqual(data.retryButtonText);
  } else {
    expect(component.find('#error').find('button').exists()).toBeFalsy();
  }
};

describe('ApiError.vue', () => {
  each(testData[403]).it('page %s will show correct message when patient does not have the necessary permissions within the GP system.', (path, expectedData) => {
    const route = { path };
    const apiError = { status: 403, message: 'Forbidden' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[409]).it('page %s will show correct message when the specified appointment does not exist or is not in a suitable state to be cancelled', (path, expectedData) => {
    const route = { path };
    const apiError = { status: 409, message: 'Conflict' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[500]).it('page %s will show correct message when unexpected error occurred processing the request in api', (path, expectedData) => {
    const route = { path };
    const apiError = { status: 500, message: 'Internal Server Error' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[502]).it('page %s will show correct message when api is currently unavailable', (path, expectedData) => {
    const route = { path };
    const apiError = { status: 502, message: 'Bad Gateway' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[504]).it('page %s will show correct message when api did not respond in a timely fashion', (path, expectedData) => {
    const route = { path };
    const apiError = { status: 504, message: 'Gateway Timeout' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });
});
