/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import each from 'jest-each';
import { mount, createLocalVue } from '@vue/test-utils';
import createStore from '@/store/index';
import ApiError from '@/components/errors/ApiError';
import locale from '@/locale';
import { get, has } from 'lodash/fp';
import testData from './testData/index';

const engLocale = locale.en;
const $te = key => has(key, engLocale);
const $t = key => get(key, engLocale);
const errorId = 'error';
let component;

const createApiErrorComponent = ($route, apiError) => {
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const store = createStore();
  store.dispatch('errors/setRoutePath', $route);
  store.dispatch('errors/addApiError', apiError);

  component = mount(ApiError, {
    store,
    localVue,
    mocks: {
      $style: { serverError: errorId, button: 'button' },
      $route,
      $t,
      $te,
      correctUrl: url => url,
    },
  });
};

const assert = (expectedData) => {
  if (expectedData.showError === false) {
    expect(component.vm.showError()).toBeDefined();
    expect(component.vm.showError()).toBeFalsy();
    return;
  }

  expect(component.vm.getPageTitle()).toEqual(expectedData.pageTitle);
  expect(component.vm.getPageHeader()).toEqual(expectedData.pageHeader);
  let paragraphIndex = -1;
  if (expectedData.isInformationError !== true) {
    expect(component.find(`#${errorId}`).findAll('p').at(paragraphIndex += 1).text()).toEqual(expectedData.header);
    if (expectedData.subheader !== '') {
      expect(component.find(`#${errorId}`).findAll('p').at(paragraphIndex += 1).text()).toEqual(expectedData.subheader);
    }
    expect(component.find(`#${errorId}`).findAll('p').at(paragraphIndex += 1).text()).toEqual(expectedData.message);
    if (expectedData.additionalInfo && expectedData.additionalInfo !== '') {
      expect(component.find(`#${errorId}`).findAll('p').at(paragraphIndex += 1).text()).toEqual(expectedData.additionalInfo);
    }
    expect(component.find(`#${errorId}`).findAll('p').length).toEqual(paragraphIndex + 1);
    if (expectedData.hasRetryButton) {
      expect(component.find(`#${errorId}`).find('.button').text()).toEqual(expectedData.retryButtonText);
      expect(component.vm.getRedirectUrl()).toEqual(expectedData.redirectUrl);
    } else {
      expect(component.find(`#${errorId}`).find('button').exists()).toBeFalsy();
    }
  } else {
    expect(component.findAll('h1').at(0).text()).toEqual(expectedData.header);
    expect(component.findAll('h2').at(0).text()).toEqual(expectedData.subheader);
    expect(component.findAll('p').at(0).text()).toEqual(expectedData.message);
  }
};

describe('ApiError.vue', () => {
  each(testData[400]).it('page %s will show correct message when API returns a 400 bad request response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 400 }, message: 'Bad Request' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[403]).it('page %s will show correct message when the API returns a 403 forbidden response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 403 }, message: 'Forbidden' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[409]).it('page %s will show correct message when the API returns a 409 conflict response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 409 }, message: 'Conflict' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[460]).it('page %s will show correct message when the API returns a 460 limit reached response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 460 }, message: 'Limit reached' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[461]).it('page %s will show correct message when the API returns a 461 too late response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 461 }, message: 'Too late' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[464]).it('page %s will show correct message when the API returns a 464 unsupported ods code or no nhs number', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 464 }, message: 'ODS code not supported or no nhs number' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[465]).it('page %s will show correct message when the API returns a 465 user did not meet the minimum age requirement', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 465 }, message: 'Underage' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });


  each(testData[500]).it('page %s will show correct message when the API returns a 500 internal server error response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 500 }, message: 'Internal Server Error' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[502]).it('page %s will show correct message when the API returns a 502 bad gateway response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 502 }, message: 'Bad Gateway' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });

  each(testData[504]).it('page %s will show correct message when the API returns a 504 gateway timeout response', (path, expectedData) => {
    const route = { path };
    const apiError = { response: { status: 504 }, message: 'Gateway Timeout' };

    createApiErrorComponent(route, apiError);

    assert(expectedData);
  });
});
