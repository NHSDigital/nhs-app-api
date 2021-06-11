import ReturnPage from '@/pages/gp-session-on-demand/return';
import i18n from '@/plugins/i18n';
import { AUTH_RETURN_PATH, TERMSANDCONDITIONS_PATH } from '@/router/paths';
import { mount, createStore } from '../../helpers';

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
  }) => {
    store = createStore({
      dispatch: jest.fn(() => Promise.resolve()),
      $env: {},
      getters: {
        'errors/showApiError': showApiError,
      },
      state: {
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
    };

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
    expect(store.dispatch).toHaveBeenCalledWith('appVersion/init');
    expect(store.dispatch).toHaveBeenCalledWith('auth/handleGpOnDemandResponse', router.currentRoute.query);
  });

  it('dispatch to terms and conditions with empty state', async () => {
    wrapper = await mountPage({ shallow: true });
    await wrapper.vm.$nextTick();
    expect(router.push).toHaveBeenCalledWith({ path: TERMSANDCONDITIONS_PATH, query: {} });
  });

  it('dispatch to terms and conditions with state', async () => {
    wrapper = await mountPage({ shallow: true, state: '/appointments' });
    await wrapper.vm.$nextTick();
    expect(router.push).toHaveBeenCalledWith({ path: TERMSANDCONDITIONS_PATH, query: { redirect_to: '/appointments' } });
  });
});
