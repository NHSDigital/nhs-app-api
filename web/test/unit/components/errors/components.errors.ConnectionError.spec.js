import ConnectionError from '@/components/errors/ConnectionError';
import * as dependency from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('ConnectionError.vue', () => {
  let wrapper;
  let $store;
  let $router;

  const createConnectionErrorComponent = ({ hasConnectionProblem }) => {
    $router = createRouter();
    $store = createStore({
      state: {
        errors: {
          hasConnectionProblem,
        },
        device: {
          isNativeApp: false,
        },
      },
    });

    return mount(ConnectionError, { $store, $router });
  };

  describe('has connection error', () => {
    beforeEach(() => {
      dependency.redirectTo = jest.fn();
      wrapper = createConnectionErrorComponent({ hasConnectionProblem: true });
    });

    it('will show error dialog', () => {
      expect(wrapper.find('#message-dialog').exists()).toBe(true);
    });

    it('will show the correct message', () => {
      const paragraphs = wrapper.findAll('p');
      expect(paragraphs.at(0).text()).toBe('Check your connection and try again.');

      const messageElement1 = paragraphs.at(1);
      expect(messageElement1.text()).toContain('If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly.');

      const messageElement2 = paragraphs.at(2);
      expect(messageElement2.text()).toContain('111.nhs.uk');
      expect(messageElement2.text()).toContain('or call 111.');
      expect(messageElement2.attributes()['aria-label']).toBe('For urgent medical advice, go to 111.nhs.uk or call one one one.');
    });

    it('will not update the header and title when there no longer is a connection error', () => {
      EventBus.$emit.mockClear();
      $store.state.errors.hasConnectionProblem = false;

      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_HEADER, 'generic.errors.internetConnectionError');
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_TITLE, 'generic.errors.internetConnectionError');
    });

    describe('try again button', () => {
      let button;

      beforeEach(() => {
        button = wrapper.find('button');
      });

      it('will exist', () => {
        expect(button.exists()).toBe(true);
      });

      describe('is proxying on click', () => {
        beforeEach(() => {
          $store.getters = { 'session/isProxying': true };
          button.trigger('click');
        });

        it('will redirect to INDEX', () => {
          expect(dependency.redirectTo).toBeCalledWith(wrapper.vm, INDEX_PATH);
        });

        it('will not refresh page', () => {
          expect($router.go).not.toBeCalled();
        });
      });

      describe('is not proxying on click', () => {
        beforeEach(() => {
          $store.getters = { 'session/isProxying': false };
          button.trigger('click');
        });

        it('will not redirect', () => {
          expect(dependency.redirectTo).not.toBeCalled();
        });

        it('will refresh page', () => {
          expect($router.go).toBeCalled();
        });
      });
    });
  });

  describe('has no connection error', () => {
    beforeEach(() => {
      wrapper = createConnectionErrorComponent({ hasConnectionProblem: false });
    });

    it('will not show error dialog', () => {
      expect(wrapper.find('#message-dialog').exists()).toBe(false);
    });

    it('will not update the header and title', () => {
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_HEADER, 'generic.errors.internetConnectionError');
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_TITLE, 'generic.errors.internetConnectionError');
    });

    it('will update the header and title when a connection error arises', () => {
      EventBus.$emit.mockClear();
      $store.state.errors.hasConnectionProblem = true;

      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'generic.errors.internetConnectionError');
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'generic.errors.internetConnectionError');
    });
  });
});
