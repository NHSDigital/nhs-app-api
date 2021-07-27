import ReturnPage from '@/pages/gp-session-on-demand/return';
import i18n from '@/plugins/i18n';
import { AUTH_RETURN_PATH, TERMSANDCONDITIONS_PATH } from '@/router/paths';
import {
  GP_PRESCRIPTION_JOURNEY_NAME,
  GP_APPOINTMENT_JOURNEY_NAME,
  GP_LINKED_ACCOUNT_JOURNEY_NAME,
  GP_HEALTH_RECORD_JOURNEY_NAME,
} from '@/router/names';
import GenericErrors from '@/components/errors/pages/on-demand-generic/GenericErrors';
import GpAppointmentGpSessionErrors from '@/components/errors/pages/appointments/GpAppointmentGpSessionErrors';
import LinkedProfileErrors from '@/components/linked-profiles/LinkedProfileErrors';
import PrescriptionErrors from '@/components/errors/pages/prescriptions/PrescriptionsErrors';
import HealthRecordErrors from '@/components/errors/pages/health-record/HealthRecordErrors';
import each from 'jest-each';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount, createStore } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn(), $on: jest.fn() },
}));

const deepLink = 'https://localhost/appointments';
const targetPage = '/appointments';

let goToUrl;
let wrapper;
let store;
let router;

const mountPage = ({
  shallow = false,
  showApiError = false,
  query = {},
  state = '',
  deepLinkUrl = undefined,
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
        deepLinkUrl,
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

describe('on-demand-gp-return', () => {
  describe('when query has deepLinkUrl', () => {
    beforeEach(async () => {
      wrapper = await mountPage({ shallow: true, state: targetPage, deepLinkUrl: deepLink });
      // We need the two calls here as there are two async calls in the code.  Newer versions
      // of the vue test utils library have a nicer way of doing this in a single call.
      await wrapper.vm.$nextTick();
      await wrapper.vm.$nextTick();
    });

    it('will navigate to terms and conditions with a redirect to the deep link url', async () => {
      expect(router.push).toHaveBeenCalledWith({
        path: TERMSANDCONDITIONS_PATH,
        query: { redirect_to: deepLink },
      });
    });

    it('will not navigate to terms and conditions with a redirect to the target page', () => {
      expect(router.push).not.toHaveBeenCalledWith({
        path: TERMSANDCONDITIONS_PATH,
        query: { redirect_to: targetPage },
      });
    });
  });

  it('will set language from locale', async () => {
    wrapper = await mountPage({ shallow: true });
    await wrapper.vm.$nextTick();
    expect(store.dispatch).toHaveBeenCalledWith('appVersion/init');
    expect(store.dispatch).toHaveBeenCalledWith('auth/handleGpOnDemandResponse', router.currentRoute.query);
  });

  it('will navigate to terms and conditions with no redirect when there is no target page', async () => {
    wrapper = await mountPage({ shallow: true });
    // We need the two calls here as there are two async calls in the code.  Newer versions
    // of the vue test utils library have a nicer way of doing this in a single call.
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();
    expect(router.push).toHaveBeenCalledWith({ path: TERMSANDCONDITIONS_PATH, query: {} });
  });

  it('will navigate to terms and conditions with a redirect to target page', async () => {
    wrapper = await mountPage({ shallow: true, state: targetPage });
    // We need the two calls here as there are two async calls in the code.  Newer versions
    // of the vue test utils library have a nicer way of doing this in a single call.
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();
    expect(router.push).toHaveBeenCalledWith({
      path: TERMSANDCONDITIONS_PATH,
      query: { redirect_to: targetPage },
    });
  });

  it('will navigate to terms and conditions with a redirect to target page when gp session error should be ignored', async () => {
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
    const journeysAndErrorComponents = [
      [GP_PRESCRIPTION_JOURNEY_NAME, PrescriptionErrors],
      [GP_APPOINTMENT_JOURNEY_NAME, GpAppointmentGpSessionErrors],
      [GP_LINKED_ACCOUNT_JOURNEY_NAME, LinkedProfileErrors],
      [GP_HEALTH_RECORD_JOURNEY_NAME, HealthRecordErrors],
      [null, GenericErrors],
    ];

    each(
      journeysAndErrorComponents,
    ).it('will display correct journey specific error', async (journeyName, componentToEnsureIsShown) => {
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

    each(
      journeysAndErrorComponents,
    ).it('will not display the error when there is a deep link to handle', async (journeyName, componentToEnsureIsShown) => {
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
      wrapper = await mountPage({ shallow: true, state: '/path', error, routeInfo, deepLinkUrl: deepLink });
      // We need the two calls here as there are two async calls in the code.  Newer versions
      // of the vue test utils library have a nicer way of doing this in a single call.
      await wrapper.vm.$nextTick();
      await wrapper.vm.$nextTick();

      expect(router.push).toHaveBeenCalledWith({
        path: TERMSANDCONDITIONS_PATH,
        query: { redirect_to: deepLink },
      });
      expect(wrapper.find(componentToEnsureIsShown).exists()).toBe(false);
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
    wrapper = await mountPage({ shallow: true, state: targetPage, routeInfo });
    await wrapper.vm.$nextTick();
    expect(EventBus.$emit).toHaveBeenCalledWith(
      UPDATE_TITLE, { titleKey: 'navigation.pages.titles.gpAppointments' },
    );
  });
});
