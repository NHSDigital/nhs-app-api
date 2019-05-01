/* eslint-disable import/no-extraneous-dependencies */
import each from 'jest-each';
import { get, has } from 'lodash/fp';
import ConnectionError from '@/components/errors/ConnectionError';
import getters from '@/store/modules/errors/getters';
import locale from '@/locale';
import { initialState } from '@/store/modules/errors/mutation-types';
import { createStore, mount } from '../../helpers';

const engLocale = locale.en;
const $te = key => has(key, engLocale);
const $t = key => get(key, engLocale);
const errorId = 'error';

const createConnectionErrorComponent = ($route, apiError) => {
  const $store = createStore({
    state: {
      errors: initialState(),
      device: {
        source: 'ios',
      },
    },
  });

  $store.state.errors.apiErrors = [apiError];
  $store.state.errors.routePath = $route.path;
  $store.state.errors.getters = getters;
  $store.state.errors.hasConnectionProblem = true;
  $store.state.device = { isNativeApp: true };

  return mount(ConnectionError, {
    $route,
    $style: { serverError: errorId, button: 'button' },
    $store,
    $te,
    $t,
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

    const component = createConnectionErrorComponent(route, apiError);
    const paragraphs = component.findAll('p');

    expect(paragraphs.at(0).text()).toBe('Connection error');
    expect(paragraphs.at(1).text()).toBe('There\'s an issue with your internet connection');

    const messageElement = paragraphs.at(2);
    expect(messageElement.text()).toBe('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.');
    expect(messageElement.attributes()['aria-label']).toBe('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.');
  });
});
