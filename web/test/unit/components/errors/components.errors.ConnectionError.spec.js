import ConnectionError from '@/components/errors/ConnectionError';
import * as dependency from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import { createRouter, createStore, mount } from '../../helpers';

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
      expect(paragraphs.at(0).text()).toBe('There is a problem with your internet connection');

      const messageElement = paragraphs.at(1);
      expect(messageElement.text()).toContain('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to');
      expect(messageElement.text()).toContain('111.nhs.uk');
      expect(messageElement.text()).toContain('or call 111.');
      expect(messageElement.attributes()['aria-label']).toBe('Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call one one one.');
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
  });
});
