import GpAppointmentErrors from '@/components/errors/pages/appointments/GpAppointmentErrors';
import i18n from '@/plugins/i18n';
import each from 'jest-each';
import { GP_APPOINTMENTS_PATH } from '@/router/paths';
import { createStore, mount, createRouter } from '../../../helpers';

describe('GpAppointmentErrors', () => {
  let $store;
  let $route;
  let $router;

  const mountPage = ({
    cdssAdviceEnabled = true,
    cdssAdminEnabled = true,
    status,
    userSessionCreateReferenceCode,
    isNativeApp = false,
    query = {},
  } = {}) => {
    $store = createStore({
      $env: {
        CONTACT_US_URL: 'https://contact.us',
      },
      state: {
        device: {
          source: 'web',
          isNativeApp,
        },
        session: {
          userSessionCreateReferenceCode,
        },
      },
      getters: {
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
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

    return mount(GpAppointmentErrors, {
      $store,
      propsData: {
        error: {
          status,
        },
      },
      computed: {
        hasConnection() {
          return true;
        },
      },
      mountOpts: { i18n, reload: jest.fn() },
      $route,
      $router,
    });
  };

  describe('errors', () => {
    let wrapper;

    each([
      400,
      500,
      502,
      504,
      599,
    ]).it('will display an error dialog for status code: %s', (status) => {
      wrapper = mountPage({ status });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });

    it('will display a server error if there is no status returned', () => {
      wrapper = mountPage();
      expect(wrapper.find('#error-dialog-unknown').exists()).toBe(true);
    });

    it('will display a server error if there is an unknown status returned', () => {
      wrapper = mountPage({ status: 600 });
      expect(wrapper.find('#error-dialog-unknown').exists()).toBe(true);
    });

    it('will set the flag in the sessionStorage when isNative app is true', async () => {
      Storage.prototype.setItem = jest.fn();

      wrapper = await mountPage({ isNativeApp: true });
      wrapper.vm.tryAgain();

      expect(sessionStorage.setItem).toBeCalledWith('hasRetried', true);
    });

    it('will not call sessionStorage when isNative app is false', async () => {
      Storage.prototype.setItem = jest.fn();

      wrapper = await mountPage();
      wrapper.vm.tryAgain();

      expect(sessionStorage.setItem).not.toBeCalled();
    });
  });
});
