import each from 'jest-each';
import GPAppointments from '@/pages/appointments/gp-appointments';
import { GP_APPOINTMENTS_PATH } from '@/router/paths';
import { mount, createStore, createRouter } from '../../helpers';


describe('index.vue', () => {
  let wrapper;
  let $store;
  let $route;
  let $router;

  const mountPage = ({ status,
    userSessionCreateReferenceCode,
    isNativeApp = false,
    query = {},
  } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp,
        },
        myAppointments: {
          error: {
            status,
          },
          hasLoaded: true,
        },
        session: {
          userSessionCreateReferenceCode,
        },
      },
      getters: {
        'session/isLoggedIn': () => true,
      },
    });

    $router = createRouter();
    $router.currentRoute = {
      path: GP_APPOINTMENTS_PATH,
    };

    $route = {
      query,
      path: GP_APPOINTMENTS_PATH,
    };

    return mount(GPAppointments, { $store, $route, $router, methods: { reload: jest.fn() } });
  };

  describe('errors', () => {
    each([
      400,
      403,
      500,
      502,
      504,
      599,
    ]).it('will display an error dialog for status code: %s', (status) => {
      wrapper = mountPage({ status });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });

    it('will dispatch the retry function if the hasRetried flag is set on the route', () => {
      wrapper = mountPage({ query: { hr: true } });
      expect($store.dispatch).toBeCalledWith('session/setRetry', true);
    });

    it('will set the flag in the sessionStorage when isNative app is true', () => {
      Storage.prototype.setItem = jest.fn();

      wrapper = mountPage({ isNativeApp: true });
      wrapper.vm.tryAgain();

      expect(sessionStorage.setItem).toBeCalledWith('hasRetried', true);
    });

    it('will not call sessionStorage when isNative app is false', () => {
      Storage.prototype.setItem = jest.fn();

      wrapper = mountPage();
      wrapper.vm.tryAgain();

      expect(sessionStorage.setItem).not.toBeCalled();
    });
  });
});
