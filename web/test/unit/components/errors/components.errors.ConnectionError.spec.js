/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import each from 'jest-each';
import { mount, createLocalVue } from '@vue/test-utils';
import createStore from '@/store/index';
import ConnectionError from '@/components/errors/ConnectionError';
import locale from '@/locale';
import { get, has } from 'lodash/fp';

const engLocale = locale.en;
const $te = key => has(key, engLocale);
const $t = key => get(key, engLocale);
const errorId = 'error';
let component;

const createApiErrorComponent = ($route, apiError) => {
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const store = createStore();
  store.dispatch('errors/setRoutePath', $route.path);
  store.dispatch('errors/addApiError', apiError);

  component = mount(ConnectionError, {
    store,
    localVue,
    mocks: {
      $style: { serverError: errorId, button: 'button' },
      $route,
      $t,
      $te,
    },
  });
};

const testDataSet = [
  '/appointments',
  '/appointments/booking-guidance',
  '/appointments/booking',
  '/appointments/confirmation',
  '/appointments/cancelling',
  '/auth-return',
  '/login',
  '/my-record',
  '/my-record/noaccess',
  '/my-record/testresultdetail/:testResultId',
  '/prescriptions',
  '/prescriptions/repeat-courses',
  '/prescriptions/confirm-prescription-details'];

describe('ConnectionError.vue', () => {
  each(testDataSet).it('page %s will show correct message when the API fails times out with no response', (path) => {
    const route = { path };
    const apiError = { message: 'API failed to return a response.' };

    createApiErrorComponent(route, apiError);

    expect(component.vm.header).toEqual('Connection error');
    expect(component.vm.subheader).toEqual('There\'s an issue with your internet connection');
    expect(component.vm.message).toEqual('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.');
    expect(component.vm.retryButtonText).toEqual('Try again');
  });
});
