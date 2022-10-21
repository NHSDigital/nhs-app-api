import Biometrics from '@/pages/biometrics-registration/index';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('biometrics prompt page', () => {
  let $router;
  let $store;
  let wrapper;
  let conditionalRedirect;

  const mountPage = ({ state }, biometricRegistered, biometricSupported = true) => {
    $router = createRouter();
    $store = createStore({ state });
    $store.getters['loginSettings/biometricType'] = 'face';
    $store.getters['loginSettings/biometricRegistered'] = biometricRegistered;
    $store.getters['loginSettings/biometricSupported'] = biometricSupported;
    $store.app.$http = {
      postV1ApiMetricsBiometricsOptIn: jest.fn(() => Promise.resolve()),
      postV1ApiMetricsBiometricsOptOut: jest.fn(() => Promise.resolve()),
    };

    conditionalRedirect = jest.fn();

    return mount(Biometrics, {
      $router,
      $store,
      stubs: {
        'no-return-flow-layout': '<div><slot/></div>',
      },
      methods: {
        conditionalRedirect,
      },
    });
  };

  beforeEach(() => {
    EventBus.$emit.mockClear();
  });

  describe('new user is prompted for biometrics', () => {
    beforeEach(() => {
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
        },
      });
    });

    describe('the user has already registered and biometrcis is supported ', () => {
      const biometricRegistered = true;
      const biometricSupported = true;

      beforeEach(() => {
        wrapper = mountPage({
          state: {
            device: {
              isNativeApp: true,
            },
          },
        },
        biometricRegistered,
        biometricSupported);
      });

      it('will call next with redirect route', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });

    describe('the user has not registered and biometrcis is not supported ', () => {
      const biometricRegistered = false;
      const biometricSupported = false;

      beforeEach(() => {
        wrapper = mountPage({
          state: {
            device: {
              isNativeApp: true,
            },
          },
        },
        biometricRegistered,
        biometricSupported);
      });

      it('will call next with redirect route', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });

    describe('the user has not registered and biometrcis is supported ', () => {
      const biometricRegistered = false;
      const biometricSupported = true;

      beforeEach(() => {
        wrapper = mountPage({
          state: {
            device: {
              isNativeApp: true,
            },
          },
        },
        biometricRegistered,
        biometricSupported);
      });

      it('will call next with redirect route', () => {
        expect(conditionalRedirect).not.toBeCalled();
      });
    });

    describe('the user does not make a choice', () => {
      beforeEach(() => {
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will show inline error', () => {
        expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(true);
      });

      it('will show form error summary', () => {
        expect(wrapper.find('#form-error-summary').exists()).toBe(true);
      });

      it('will not dispatch', () => {
        expect($store.dispatch).not.toBeCalled();
      });

      it('will not call conditional redirect', () => {
        expect(conditionalRedirect).not.toBeCalled();
      });

      it('will set focus on the error component', () => {
        expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
      });

      describe('choice made', () => {
        beforeEach(() => {
          wrapper.find('#biometricsRegistration-true').setChecked();
        });

        it('will not show inline error', () => {
          expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(false);
        });

        it('will not show error dialog', () => {
          expect(wrapper.find('#message-dialog').exists()).toBe(false);
        });
      });
    });

    describe('the user agreed to biometrics', () => {
      beforeEach(() => {
        wrapper.find('#biometricsRegistration-true').setChecked();
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will not show inline error', () => {
        expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(false);
      });

      it('will not show error dialog', () => {
        expect(wrapper.find('#message-dialog').exists()).toBe(false);
      });

      it('will not set focus on the error component', () => {
        expect(EventBus.$emit).not.toBeCalledWith(FOCUS_ERROR_ELEMENT);
      });

      it('will add the cookie', () => {
        expect($store.dispatch).toBeCalledWith('biometrics/addBiometricsCookie');
      });

      it('will call loginSettings/updateRegistration', () => {
        expect($store.dispatch).toBeCalledWith('loginSettings/updateRegistration');
      });
    });

    describe('the user does not agree to biometrcis', () => {
      beforeEach(() => {
        wrapper.find('#biometricsRegistration-false').setChecked();
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will not show error dialog', () => {
        expect(wrapper.find('#message-dialog').exists()).toBe(false);
      });

      it('will not set focus on the error component', () => {
        expect(EventBus.$emit).not.toBeCalledWith(FOCUS_ERROR_ELEMENT);
      });

      it('will add the cookie', () => {
        expect($store.dispatch).toBeCalledWith('biometrics/addBiometricsCookie');
      });

      it('will call postV1ApiMetricsBiometricsOptOut', () => {
        expect($store.app.$http.postV1ApiMetricsBiometricsOptOut).toBeCalled();
      });

      it('will call next with redirect route', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });
  });
});
