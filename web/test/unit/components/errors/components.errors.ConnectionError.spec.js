/* eslint-disable import/no-extraneous-dependencies */
import each from 'jest-each';
import get from 'lodash/fp/get';
import has from 'lodash/fp/has';
import ConnectionError from '@/components/errors/ConnectionError';
import getters from '@/store/modules/errors/getters';
import locale from '@/locale';
import { createStore, mount } from '../../helpers';

const engLocale = locale.en;
const $te = key => has(key, engLocale);
const $t = key => get(key, engLocale);
const errorId = 'error';

let $store;

const createConnectionErrorComponent = ({
  $route = { path: '/' },
  hasConnectionProblem = true,
} = {}) => {
  $store = createStore({
    state: {
      errors: {
        apiErrors: [{ message: 'API failed to return a response.' }],
        routePath: $route.path,
        hasConnectionProblem,
        getters,
      },
      device: { isNativeApp: false },
    },
  });

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
  '/appointments/booking',
  '/appointments/confirmation',
  '/appointments/cancelling',
  '/auth-return',
  '/login',
  '/prescriptions',
  '/prescriptions/repeat-courses',
  '/prescriptions/confirm-prescription-details'];

describe('ConnectionError.vue', () => {
  each(testDataSet).it('page %s will show correct message when the API fails times out with no response', (path) => {
    const $route = { path };

    const component = createConnectionErrorComponent({ $route });
    const paragraphs = component.findAll('p');

    expect(paragraphs.at(0).text()).toBe('There is a problem with your internet connection');

    const messageElement = paragraphs.at(1);
    expect(messageElement.text()).toContain('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to');
    expect(messageElement.text()).toContain('111.nhs.uk');
    expect(messageElement.text()).toContain('or call 111.');
    expect(messageElement.attributes()['aria-label']).toBe('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call one one one.');
  });
});
