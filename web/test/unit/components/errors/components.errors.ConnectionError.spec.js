/* eslint-disable import/no-extraneous-dependencies */
import each from 'jest-each';
import get from 'lodash/fp/get';
import has from 'lodash/fp/has';
import ConnectionError from '@/components/errors/ConnectionError';
import getters from '@/store/modules/errors/getters';
import locale from '@/locale';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

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
  '/appointments/booking-guidance',
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
    expect(messageElement.text()).toBe('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.');
    expect(messageElement.attributes()['aria-label']).toBe('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.');
  });

  describe('updated', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
    });

    it('will not emit to EventBus when updated and no connection error', () => {
      createConnectionErrorComponent();
      $store.state.errors.hasConnectionProblem = false;

      expect(EventBus.$emit).not.toHaveBeenCalled();
    });

    it('will update header and title when updated and connection error shown', () => {
      createConnectionErrorComponent({ hasConnectionProblem: false });
      $store.state.errors.hasConnectionProblem = true;

      expect(EventBus.$emit).toHaveBeenCalledTimes(2);
      expect(EventBus.$emit).toHaveBeenNthCalledWith(1, UPDATE_HEADER, 'noConnection.header');
      expect(EventBus.$emit).toHaveBeenNthCalledWith(2, UPDATE_TITLE, 'noConnection.header');
    });
  });
});
