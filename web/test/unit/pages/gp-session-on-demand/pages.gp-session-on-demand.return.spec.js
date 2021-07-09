import ReturnPage from '@/pages/gp-session-on-demand/return';
import i18n from '@/plugins/i18n';
import { AUTH_RETURN_PATH, TERMSANDCONDITIONS_PATH } from '@/router/paths';
import {
  GP_PRESCRIPTION_JOURNEY_NAME,
  GP_APPOINTMENT_JOURNEY_NAME,
  GP_LINKED_ACCOUNT_JOURNEY_NAME,
} from '@/router/names';
import GenericErrors from '@/components/errors/pages/on-demand-generic/GenericErrors';
import GpAppointmentGpSessionErrors from '@/components/errors/pages/appointments/GpAppointmentGpSessionErrors';
import LinkedProfileErrors from '@/components/linked-profiles/LinkedProfileErrors';
import PrescriptionErrors from '@/components/errors/pages/prescriptions/PrescriptionsErrors';
import each from 'jest-each';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount, createStore } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn(), $on: jest.fn() },
}));

describe('on-demand-gp-return', () => {
  let goToUrl;
  let wrapper;
  let store;
  let router;

  const mountPage = ({
    shallow = false,
    showApiError = false,
    query = {},
    state = '',
    error = null,
    routeInfo = null,
  }) => {
    store = createStore({
      dispatch: jest.fn(() => Promise.resolve()),
      $env: {},
      getters: {
        'errors/showApiError': showApiError,
      },
      state: {
        auth: {
          gpSessionError: error,
        },
        appVersion: {
          nativeVersion: true,
        },
        device: { isNativeApp: true },
        errors: {
          pageSettings: { errorOverrideStyles: [] },
          routePath: AUTH_RETURN_PATH,
          apiErrors: [],
        },
      },
    });
    router = {
      currentRoute: {
        query: {
          code: 'randomcodefromnhslogin',
          state,
        },
      },
      push: jest.fn(),
      resolve: jest.fn().mockImplementation(() => routeInfo),
    };

    Storage.prototype.setItem = jest.fn();

    return mount(ReturnPage, {
      shallow,
      mocks: {
        correctUrl: jest.fn(),
        goToUrl,
      },
      $store: store,
      $route: {
        query,
      },
      $router: router,
      mountOpts: { i18n },
    });
  };

  it('will set language from locale', async () => {
    wrapper = await mountPage({ shallow: true });
    await wrapper.vm.$nextTick();
    expect(store.dispatch).toHaveBeenCalledWith('appVersion/init');
    expect(store.dispatch).toHaveBeenCalledWith('auth/handleGpOnDemandResponse', router.currentRoute.query);
  });

  it('dispatch to terms and conditions with empty state', async () => {
    wrapper = await mountPage({ shallow: true });
    // We need the two calls here as there are two async calls in the code.  Newer versions
    // of the vue test utils library have a nicer way of doing this in a single call.
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();
    expect(router.push).toHaveBeenCalledWith({ path: TERMSANDCONDITIONS_PATH, query: {} });
  });

  it('dispatch to terms and conditions with state', async () => {
    wrapper = await mountPage({ shallow: true, state: '/appointments' });
    // We need the two calls here as there are two async calls in the code.  Newer versions
    // of the vue test utils library have a nicer way of doing this in a single call.
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();
    expect(router.push).toHaveBeenCalledWith({ path: TERMSANDCONDITIONS_PATH, query: { redirect_to: '/appointments' } });
  });

  it('will redirect to target page when gp session error should be ignored', async () => {
    const error = { serviceDeskReference: '' };
    const routeInfo = {
      route: {
        meta: {
          gpSessionOnDemand: {
            ignoreError: true,
          },
        },
      },
    };
    wrapper = await mountPage({ shallow: true, state: '/path', error, routeInfo });
    // We need the two calls here as there are two async calls in the code.  Newer versions
    // of the vue test utils library have a nicer way of doing this in a single call.
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();

    expect(sessionStorage.setItem).not.toBeCalled();
    expect(router.push).toHaveBeenCalledWith({ path: TERMSANDCONDITIONS_PATH, query: { redirect_to: '/path' } });
  });

  describe('displaying journey specific help', () => {
    each([
      [GP_PRESCRIPTION_JOURNEY_NAME, PrescriptionErrors],
      [GP_APPOINTMENT_JOURNEY_NAME, GpAppointmentGpSessionErrors],
      [GP_LINKED_ACCOUNT_JOURNEY_NAME, LinkedProfileErrors],
      [null, GenericErrors],
    ])
      .it('will display correct journey specific error', async (journeyName, componentToEnsureIsShown) => {
        const error = { serviceDeskReference: '' };
        const routeInfo = {
          route: {
            meta: {
              gpSessionOnDemand: {
                journey: journeyName,
              },
            },
          },
        };
        wrapper = await mountPage({ shallow: true, state: '/path', error, routeInfo });
        // We need the two calls here as there are two async calls in the code.  Newer versions
        // of the vue test utils library have a nicer way of doing this in a single call.
        await wrapper.vm.$nextTick();
        await wrapper.vm.$nextTick();

        expect(router.push).not.toHaveBeenCalled();
        expect(wrapper.find(componentToEnsureIsShown).exists()).toBe(true);
      });
  });

  it('will dispatch to update title', async () => {
    const routeInfo = {
      route: {
        meta: {
          titleKey: 'navigation.pages.titles.gpAppointments',
        },
      },
    };
    wrapper = await mountPage({ shallow: true, state: '/appointments', routeInfo });
    await wrapper.vm.$nextTick();
    expect(EventBus.$emit).toHaveBeenCalledWith(
      UPDATE_TITLE, { titleKey: 'navigation.pages.titles.gpAppointments' },
    );
  });
});
